const submitBtn = document.getElementById("submit-btn");
const errorMessage = document.getElementById("error-message");
const success = document.querySelector(".mess");
submitBtn.addEventListener("click", () => {
    const name = document.getElementById("name").value.trim();
    const email = document.getElementById("email").value.trim();
    const subject = document.getElementById("subject").value.trim();
    const message = document.getElementById("message").value.trim();

    if (!name || !email || !subject || !message) {
        errorMessage.innerHTML = "Please complete all information";
    } else {
        errorMessage.innerHTML = "";
        success.style.display = "block";
    }
});