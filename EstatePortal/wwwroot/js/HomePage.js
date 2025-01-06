let text = "Welcome to Manufacture";
let index = 0;
let typingText = document.getElementById('typingText');

function typeWriter() {
    if (index < text.length) {
        typingText.innerHTML += text.charAt(index);
        index++;
        setTimeout(typeWriter, 200);
    }
}

window.onload = function () {
    document.getElementById("tlo").style.backgroundImage = "url('/img/ProbaPrzyciemnienie1.jpg')";
    typeWriter();
};
/* Utrzymuje sie "active" przy przyciskach Szukaj oraz oferty deweloperów*/
document.addEventListener("DOMContentLoaded", function () {

    const buttons = document.querySelectorAll(".tabs button");
    buttons.forEach(function (button) {
        button.addEventListener("click", function () {
            buttons.forEach(function (btn) {
                btn.classList.remove("active");
            });
            this.classList.add("active");
        });
    });
});
/*Stylizacja rozsuwanego menu*/
document.addEventListener("DOMContentLoaded", function () {
    const dropdowns = document.querySelectorAll(".dropdown");

    dropdowns.forEach(function (dropdown) {
        const button = dropdown.querySelector(".dropdown-button");
        const menuItems = dropdown.querySelectorAll(".dropdown-menu li");

        button.addEventListener("click", function (event) {
            event.stopPropagation();
            dropdown.classList.toggle("active");
        });

        menuItems.forEach(function (item) {
            item.addEventListener("click", function () {
                button.textContent = item.textContent;
                dropdown.classList.remove("active");
            });
        });
    });
    document.addEventListener("click", function () {
        dropdowns.forEach(function (dropdown) {
            dropdown.classList.remove("active");
        });
    });
});

/* karuzela */
document.addEventListener("DOMContentLoaded", function () {
    const container = document.querySelector(".ogloszenia-container");
    const arrowLeft = document.querySelector(".arrow-left");
    const arrowRight = document.querySelector(".arrow-right");

    let scrollAmount = 0;
    const scrollStep = 400; // Ilość przesuwania w px za każdym razem

    // Funkcja przesuwania w lewo
    arrowLeft.addEventListener("click", function () {
        if (scrollAmount > 0) {
            scrollAmount -= scrollStep;
            container.scrollTo({
                left: scrollAmount,
                behavior: "smooth",
            });
        }
    });

    // Funkcja przesuwania w prawo
    arrowRight.addEventListener("click", function () {
        if (scrollAmount < container.scrollWidth - container.clientWidth) {
            scrollAmount += scrollStep;
            container.scrollTo({
                left: scrollAmount,
                behavior: "smooth",
            });
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const searchTab = document.getElementById("searchTab");
    const developerTab = document.getElementById("developerTab");
    const searchSection = document.getElementById("search-section");
    const developerSection = document.getElementById("developer-section");
    const searchContent = document.getElementById("searchContent");
    const developerContent = document.getElementById("developerContent");
    const tlo = document.getElementById("tlo");


    function activateTab(tab) {

        searchSection.classList.remove("active");
        developerSection.classList.remove("active");
        searchContent.classList.remove("active");
        developerContent.classList.remove("active");

        if (tab === "search") {
            searchSection.classList.add("active");
            searchContent.classList.add("active");
            searchTab.classList.add("active");
            developerTab.classList.remove("active");
            tlo.style.backgroundImage = "url('/img/ProbaPrzyciemnienie1.jpg')";
        } else if (tab === "developer") {
            developerSection.classList.add("active");
            developerContent.classList.add("active");
            developerTab.classList.add("active");
            searchTab.classList.remove("active");
            tlo.style.backgroundImage = "url('/img/Gdansk.jpeg')";
        }
    }

    searchTab.addEventListener("click", function () {
        activateTab("search");
    });

    developerTab.addEventListener("click", function () {
        activateTab("developer");
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const buttons = document.querySelectorAll(".rodzaje-container button");
    buttons.forEach(function (button) {
        button.addEventListener("click", function () {
            buttons.forEach(function (btn) {
                btn.classList.remove("active");
            });
            this.classList.add("active");
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    // Formularz wyszukiwania
    const searchForm = document.getElementById("searchForm");
    const searchButton = searchForm.querySelector(".search-button");

    searchButton.addEventListener("click", function (event) {
        event.preventDefault(); // Zatrzymujemy domyślną akcję wysyłania formularza

        // Przypisanie wartości domyślnych, jeśli pola są puste
        const inputs = searchForm.querySelectorAll("input");

        inputs.forEach(input => {
            if (!input.value) {
                if (input.name === "location") {
                    input.value = "Warszawa";
                } else if (input.name === "priceMin") {
                    input.value = "1000";
                } else if (input.name === "priceMax") {
                    input.value = "5000";
                } else if (input.name === "surfaceMin") {
                    input.value = "30";
                } else if (input.name === "surfaceMax") {
                    input.value = "150";
                }
            }
        });

        // Po przypisaniu wartości domyślnych wysyłamy formularz
        searchForm.submit();
    });

    // Formularz deweloperów
    const developerForm = document.getElementById("developerForm");
    const developerButton = developerForm.querySelector(".search-button");

    developerButton.addEventListener("click", function (event) {
        event.preventDefault();  // Zatrzymujemy domyślną akcję formularza

        const developerInputs = developerForm.querySelectorAll("input");

        developerInputs.forEach(input => {
            if (!input.value) {
                if (input.name === "location") {
                    input.value = "Warszawa";
                } else if (input.name === "priceMin") {
                    input.value = "1000";
                } else if (input.name === "priceMax") {
                    input.value = "5000";
                } else if (input.name === "surfaceMin") {
                    input.value = "30";
                } else if (input.name === "surfaceMax") {
                    input.value = "150";
                }
            }
        });

        // Po nadaniu domyślnych wartości wysyłamy formularz
        developerForm.submit();
    });

    // Obsługa rozwijanych menu
    const dropdowns = document.querySelectorAll(".dropdown");

    dropdowns.forEach(function (dropdown) {
        const button = dropdown.querySelector(".dropdown-button");
        const menu = dropdown.querySelector(".dropdown-menu");

        button.addEventListener("click", function (event) {
            event.stopPropagation(); // Zatrzymuje propagację kliknięcia, aby nie wysłać formularza
            menu.classList.toggle("active"); // Zmieniamy klasę, aby wyświetlić menu
        });

        // Ukrywa menu, jeśli klikniesz poza nim
        document.addEventListener("click", function (event) {
            // Jeśli kliknięto poza dropdown, ukryj menu
            if (!dropdown.contains(event.target)) {
                menu.classList.remove("active");
            }
        });
    });

});

document.addEventListener("DOMContentLoaded", function () {
    const sellButton = document.getElementById("sell");
    const rentButton = document.getElementById("rent");
    const sellGrid = document.getElementById("sell-grid");
    const rentGrid = document.getElementById("rent-grid");

    sellButton.addEventListener("click", function () {
        sellButton.classList.add("active");
        rentButton.classList.remove("active");

        sellGrid.style.display = "grid";
        rentGrid.style.display = "none";
    });

    rentButton.addEventListener("click", function () {
        rentButton.classList.add("active");
        sellButton.classList.remove("active");

        rentGrid.style.display = "grid";
        sellGrid.style.display = "none";
    });
});



