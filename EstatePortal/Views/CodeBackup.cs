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