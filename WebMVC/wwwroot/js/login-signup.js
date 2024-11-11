const inputPass = document.querySelector(".input-info .password");
const eyePass = document.querySelector(".passeye .fa-eye-slash");
const eyeShow = document.querySelector(".passeye .fa-eye");

eyePass.addEventListener("click", () => {
    eyePass.classList.add("active");
    eyeShow.classList.remove("active");
    inputPass.setAttribute("type", "text");
});

eyeShow.addEventListener("click", () => {
    eyePass.classList.remove("active");
    eyeShow.classList.add("active");
    inputPass.setAttribute("type", "password");
});
