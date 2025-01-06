using System;
using System.IO;
using System.Linq;
using Microsoft.ML.Transforms.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.ML.Data;

namespace EstatePortal.Services
{
    /// <summary>
    /// Serwis do obsługi modelu Object Detection (wygenerowanego przez ML.NET Model Builder)
    /// który jest rozbity na MLModel.training.cs i MLModel.consumption.cs
    /// i znajduje się w klasie partial MLModel.
    /// 
    /// Główna metoda: CheckForbiddenObjects(Stream) - zwraca najwyższy confidence spośród wykrytych obiektów.
    /// </summary>
    public class ImageDetectionService
    {
        /// <summary>
        /// Analizuje obraz pod kątem wykrytych obiektów i zwraca najwyższe Score (confidence).
        /// Docelowo możesz interpretować to jako "prawdopodobieństwo" niedozwolonego obiektu.
        /// </summary>
        /// <param name="imageStream">Strumień z wgranego pliku (np. file.OpenReadStream())</param>
        /// <returns>Maksymalne Score (0..1) spośród wykrytych obiektów, albo 0 jeśli nic nie wykryto</returns>
        public float CheckForbiddenObjects(Stream imageStream)
        {
            // 1. Zapis do pliku tymczasowego,
            //    ponieważ w MLModel.consumption.cs widać, że 'ModelInput' oczekuje MLImage, 
            //    a MLImage najprościej utworzyć z pliku: MLImage.CreateFromFile(path).
            string tempFilePath = SaveStreamToTempFile(imageStream);

            try
            {
                // 2. Utwórz MLImage z tego pliku
                var mlImage = MLImage.CreateFromFile(tempFilePath);

                // 3. Stwórz obiekt ModelInput
                //    (zgodnie z definicją w MLModel.consumption.cs)
                var input = new EstatePortal.MLModel.ModelInput
                {
                    // MLImage (800x600) - tu trafia nasz obraz
                    Image = mlImage,

                    // Ponieważ to predykcja, nie mamy w tym momencie "Labels" czy "Box"
                    // (one były w trakcie treningu). 
                    // Ale klasa wymaga tych pól, więc dajemy puste tablice:
                    Labels = Array.Empty<string>(),
                    Box = Array.Empty<float>(),
                };

                // 4. Wywołaj metodę Predict z kl. MLModel
                //    (patrz MLModel.consumption.cs => public static ModelOutput Predict(ModelInput input))
                var prediction = EstatePortal.MLModel.Predict(input);

                // 5. W obiekcie 'prediction' mamy m.in. 
                //      - Score (float[]) 
                //      - PredictedLabel (string[]) 
                //      - PredictedBoundingBoxes (float[])
                //
                // Score[i] to confidence dla i-tego wykrytego obiektu (w bounding boxie).
                // Znajdźmy najwyższe.
                float maxScore = 0f;
                if (prediction.Score != null && prediction.Score.Length > 0)
                {
                    maxScore = prediction.Score.Max();
                }

                // 6. Zwracamy najwyższe Score
                return maxScore;
            }
            finally
            {
                // 7. Posprzątanie pliku tymczasowego
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        /// <summary>
        /// Zapisuje strumień do tymczasowego pliku .jpg i zwraca ścieżkę do niego
        /// </summary>
        private string SaveStreamToTempFile(Stream imageStream)
        {
            string tempFile = Path.GetTempFileName() + ".jpg";
            using var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write);
            imageStream.Seek(0, SeekOrigin.Begin);
            imageStream.CopyTo(fs);
            return tempFile;
        }
    }
}
