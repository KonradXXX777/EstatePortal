// Add Employee - stary i działający kod
//[HttpPost]
//public async Task<IActionResult> InviteEmployee(string email)
//{
//    var userId = User.FindFirst("UserId")?.Value;
//    if (string.IsNullOrEmpty(userId))
//    {
//        return RedirectToAction("Login");
//    }

//    var employer = await _context.Users.FindAsync(int.Parse(userId));
//    if (employer == null || (employer.Role != UserRole.EstateAgency && employer.Role != UserRole.Developer))
//    {
//        ModelState.AddModelError("", "Brak uprawnień do zapraszania pracowników.");
//        return View("UserPanel");
//    }

//    // Generowanie tokena
//    var token = Guid.NewGuid().ToString();

//    // Zapisanie tokena do użytkownika (jeśli model `User` zawiera miejsce na przechowywanie tokena)
//    employer.InvitationToken = token; // Zmieniono na InvitationToken
//    await _context.SaveChangesAsync();

//    // Tworzenie linku z tokenem
//    var registrationLink = Url.Action("EmployeeRegister", "Account", new { token = employer.InvitationToken, employerId = employer.Id }, Request.Scheme);

//    // Wysłanie wiadomości e-mail
//    await SendInvitationEmail(email, registrationLink);

//    ViewBag.Message = "Zaproszenie zostało wysłane.";
//    return View("UserPanel");
//}

//---------------------------------------------------------

//// Employee register
//using EstatePortal.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//[HttpGet]
//public async Task<IActionResult> EmployeeRegister(string InvitationToken, int employerId)
//{
//    //// Sprawdzenie, czy dane z URL są poprawne
//    //if (string.IsNullOrEmpty(InvitationToken) || employerId <= 0)
//    //{
//    //    return BadRequest("Nieprawidłowe dane rejestracji.");
//    //}

//    //// Weryfikacja, czy pracodawca istnieje i czy token jest poprawny
//    //var employer = await _context.Users
//    //    .FirstOrDefaultAsync(u => u.Id == employerId && u.InvitationToken == InvitationToken);

//    //if (employer == null)
//    //{
//    //    return BadRequest("Nieprawidłowe dane rejestracji lub zaproszenie wygasło.");
//    //}

//    // Przekazanie tokena i employerId do widoku rejestracji
//    var model = new EmployeeRegister
//    {
//        InvitationToken = InvitationToken,
//        Email = "", // Email może być wypełniony, jeśli logika tego wymaga
//    };

//    ViewBag.EmployerId = employerId;
//    return View("~/Views/Home/EmployeeRegister.cshtml", model);

//}

//------------------------------------------

//[HttpPost]
//public async Task<IActionResult> EmployeeRegister(EmployeeRegister model, string InvitationToken, int employerId)
//{
//    // Walidacja danych formularza
//    if (ModelState.IsValid)
//    {
//        ModelState.AddModelError("", "Model jest nieprawidłowy.");
//        return View("~/Views/Home/EmployeeRegister.cshtml", model);


//    }

//    // Weryfikacja, czy pracodawca istnieje i czy token jest poprawny
//    var employer = await _context.Users
//        .FirstOrDefaultAsync(u => u.Id == employerId && u.InvitationToken == InvitationToken);

//    if (employer != null)
//    {
//        ModelState.AddModelError("", "Nieprawidłowe lub wygasłe zaproszenie.");
//        return View("~/Views/Home/EmployeeRegister.cshtml", model);

//    }

//    // Tworzenie nowego użytkownika jako pracownika
//    var salt = GenerateSalt();
//    var newUser = new User
//    {
//        Email = model.Email,
//        PasswordHash = HashPassword(model.PasswordHash, salt),
//        PasswordSalt = salt,
//        Role = UserRole.Employee,
//        FirstName = model.FirstName,
//        LastName = model.LastName,
//        PhoneNumber = model.PhoneNumber,
//        EmployerId = employer.Id, // Powiązanie nowego pracownika z pracodawcą
//        DateRegistered = DateTime.Now,
//        AcceptTerms = model.AcceptTerms,
//    };

//    _context.Users.Add(newUser);
//    await _context.SaveChangesAsync();

//    // Reset tokena zaproszenia po jego wykorzystaniu
//    employer.InvitationToken = null;
//    _context.Users.Update(employer);
//    await _context.SaveChangesAsync();

//    return RedirectToAction("Login", "Home");
//}

//-------------------------------

// Stary AddApartment


//using Microsoft.CodeAnalysis.Elfie.Serialization;
//using Microsoft.CodeAnalysis.Scripting;
//using static System.Collections.Specialized.BitVector32;
//using System.ComponentModel;
//using System.Drawing;
//using System.Xml.Linq;

//@model EstatePortal.Models.Announcement

//< !DOCTYPE html >
//< html lang = "pl" >
//< head >

//	< meta charset = "UTF-8" >

//	< meta name = "viewport" content = "width=device-width, initial-scale=1.0" >

//	< title > Dodaj Ogłoszenie </ title >

//	< script src = "https://maps.googleapis.com/maps/api/js?key=AIzaSyAv_iuStXOtfssDcTMwz4YaMtGGiUXFeXY&libraries=places" ></ script >

//	< style >
//			/* Styl dla całego dokumentu i widżetu mapy */
//# map {
//			height: 500px;
//width: 100 %;
//		}

//		.search - container {
//position: absolute;
//top: 10px;
//left: 50 %;
//transform: translateX(-50 %);
//	z - index: 5;
//background: white;
//	border - radius: 5px;
//	box - shadow: 0 2px 5px rgba(0,0,0,0.3);
//padding: 10px;
//}

//			.search - container input {
//				width: 300px;
//padding: 10px;
//font - size: 16px;
//border: 1px solid #ddd;
//				border-radius: 3px;
//			}

//	</ style >
//</ head >
//< body >

//	< h2 > Dodaj Ogłoszenie </ h2 >


//	< !--Formularz tworzenia ogłoszenia -->

//	<form asp-action="AddApartment" method="post" enctype="multipart/form-data">
//		<div class="form-group">
//			<label asp-for="Title"></label>
//			<input asp-for="Title" class="form-control" />
//			<span asp-validation-for="Title" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label asp-for="Price"></label>
//			<input asp-for="Price" type="number" class="form-control" inputmode="numeric" />
//			<span asp-validation-for="Price" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label asp-for="Type"></label>
//			<select asp-for="Type" class="form-control" asp-items="Html.GetEnumSelectList<PropertyType>()"></select>
//			<span asp-validation-for="Type" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label asp-for="Area"></label>
//			<input asp-for="Area" type="number" class="form-control" inputmode="numeric" />
//			<span asp-validation-for="Area" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label asp-for="YouTubeVideoUrl"></label>
//			<input asp-for="YouTubeVideoUrl" class="form-control" />
//			<span asp-validation-for="YouTubeVideoUrl" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label asp-for="City">Miejscowość</label>
//			<input asp-for="City" class="form-control" id="searchInput" />
//		</div>

//		<div class="form-group">
//			<label asp-for="District"></label>
//			<input asp-for="District" class="form-control" />
//			<span asp-validation-for="District" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label asp-for="Street"></label>
//			<input asp-for="Street" class="form-control" />
//			<span asp-validation-for="Street" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label asp-for="Description"></label>
//			<textarea asp-for="Description" class="form-control" id="Description" rows="5"></textarea>
//			<span asp-validation-for="Description" class="text-danger"></span>
//		</div>

//		<div class="form-group">
//			<label>Multimedia</label>
//			<input type="file" class="form-control" name="MultimediaFiles" multiple id="multimediaInput" accept="image/*" />
//			<div id="imagePreview" style="margin-top: 10px; display: flex; flex-wrap: wrap;"></div>
//		</div>

//		<button type="submit" class="btn btn-primary">Dodaj ogłoszenie</button>
//	</form>


//< !-- Google Maps i wyszukiwanie -->
//<div id="map"></div>
//<!-- To pole jest ukryte, ale nadal dostępne dla autouzupełniania -->
//<div class= "search-container" style = "display: none;" >

//	< input id = "searchInput" type = "text" placeholder = "Wpisz miejscowość" />

//</ div >


//< script >
//	function initMap() {
//// Inicjalizacja mapy
//const map = new google.maps.Map(document.getElementById("map"), {
//			center: { lat: 52.409538, lng: 16.931992 },  // Ustawienie domyślnego środka mapy
//			zoom: 13,
//			mapTypeControl: false

//		});

//// Inicjalizacja autouzupełniania miejsc
//const input = document.getElementById("searchInput");
//const autocomplete = new google.maps.places.Autocomplete(input);
//autocomplete.bindTo("bounds", map);

//// Ustawienie znacznika
//const marker = new google.maps.Marker({
//			map: map,
//anchorPoint: new google.maps.Point(0, -29)
//		});

//// Event: gdy użytkownik wybierze miejsce z autouzupełniania
//autocomplete.addListener("place_changed", () => {
//marker.setVisible(false);
//const place = autocomplete.getPlace();

//if (!place.geometry || !place.geometry.location)
//{
//	alert("Nie można znaleźć miejsca dla: '" + place.name + "'");
//	return;
//}

//// Ustawienie mapy na wybrany obszar
//if (place.geometry.viewport)
//{
//	map.fitBounds(place.geometry.viewport);
//}
//else
//{
//	map.setCenter(place.geometry.location);
//	map.setZoom(17);
//}

//// Ustawienie znacznika na wybrane miejsce
//marker.setPosition(place.geometry.location);
//marker.setVisible(true);

//// Ustawienie wartości miasta w formularzu
//document.getElementById("searchInput").value = place.name;
//});
//	}

//	// Inicjalizacja mapy po załadowaniu strony
//	window.onload = initMap;

//</ script >

//@section Scripts {
//	<!-- jQuery -->
//	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>

//	<!-- TinyMCE -->
//	<script src="https://cdnjs.cloudflare.com/ajax/libs/tinymce/5.10.2/tinymce.min.js"></script>
//	<script>
//		tinymce.init({
//			selector: '#Description',
//			plugins: 'advlist autolink lists link image charmap print preview hr anchor pagebreak',
//			menubar: false,
//			toolbar: 'undo redo | bold italic underline | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | removeformat',
//			height: 300
//		});

//	</ script >


//	< !--Skrypt podglądu załadowanych obrazów -->
//	<script>
//		document.getElementById('multimediaInput').addEventListener('change', function (event) {
//			const imagePreview = document.getElementById('imagePreview');
//imagePreview.innerHTML = ''; // Wyczyść poprzednie miniatury

//const files = event.target.files;

//Array.from(files).forEach(file => {
//if (file && file.type.startsWith('image/'))
//{
//	const reader = new FileReader();

//	reader.onload = function(e) {
//		const imgElement = document.createElement('img');
//		imgElement.src = e.target.result;
//		imgElement.style.width = '100px';
//		imgElement.style.height = '100px';
//		imgElement.style.objectFit = 'cover';
//		imgElement.style.marginRight = '10px';
//		imgElement.style.marginBottom = '10px';
//		imgElement.alt = 'Preview';

//		imagePreview.appendChild(imgElement);
//	};

//	reader.readAsDataURL(file);
//}
//});
//		});

//	</ script >
//}
//</ body >
//</ html >