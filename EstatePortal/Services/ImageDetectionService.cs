using System;
using System.IO;
using System.Linq;
using Microsoft.ML.Transforms.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.ML.Data;

namespace EstatePortal.Services
{
    public class ImageDetectionService
    {
        public float CheckForbiddenObjects(Stream imageStream)
        {
            string tempFilePath = SaveStreamToTempFile(imageStream);

            try
            {
                var mlImage = MLImage.CreateFromFile(tempFilePath);
                var input = new EstatePortal.MLModel.ModelInput
                {
                    Image = mlImage,
                    Labels = Array.Empty<string>(),
                    Box = Array.Empty<float>(),
                };
                var prediction = EstatePortal.MLModel.Predict(input);
                float maxScore = 0f;
                if (prediction.Score != null && prediction.Score.Length > 0)
                {
                    maxScore = prediction.Score.Max();
                }
                return maxScore;
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
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
