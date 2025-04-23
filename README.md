# EstatePortal

A real estate advertising portal project using a Microsoft© machine learning model to detect unwanted objects in photos.

## Technologies
- C# 
- ASP .Net Core 8 MVC
- Entity Framework
- SignalR
- ML .NET
- MySQL
- Xampp

## 🛠️ Configuration

1. Import the database `mybase.csv` (you can choose e.g. Xampp) or create new one and set up your own database connection in `appsettings.json`.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=mybase;User=root;"
}
  ```
If you create new database: 
- delete old migrations
- create a new one with `add-migration` command
- and `update-database`

2. Enter the SMTP server settings

```json
"SmtpSettings": {
  "Host": "sandbox.smtp.mailtrap.io",
  "Port": 587,
  "SenderName": "Estate Portal",
  "SenderEmail": "YOUR_EMAIL",
  "Username": "YOUR_USERNAME",
  "Password": "YOUR_PASSWORD",
  "EnableSsl": true
}
```

3. To integrate the app with Google Maps and the autocomplete city name and Google Maps feature, add the API key in the `AddAnnouncement.cshtml` file.

```html
<script src="https://maps.googleapis.com/maps/api/js?key=YOUR_KEY&libraries=places"></script>
```