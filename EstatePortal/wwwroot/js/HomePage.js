let text = "Welcome to Manufacture";
let index = 0;
let typingText = document.getElementById('typingText');

function typeWriter()
{
    if (index < text.length)
    {
        typingText.innerHTML += text.charAt(index);
        index++;
        setTimeout(typeWriter, 200);
    }
}


window.onload = typeWriter;

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
