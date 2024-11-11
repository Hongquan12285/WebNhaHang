const button4 = document.querySelector(".group-info .btn4");
const error = document.querySelector(".content-book .error");
console.log(button4);
button4.addEventListener("click", function() {
    const bookPerson = document.querySelector(" .group1");
    const bookDay = document.querySelector(".day");
    const time = document.querySelector(" .time");
    const mess = document.querySelector("#mess");
    const input = document.querySelector("#name");
    const input2 = document.querySelector("#email");
    if (bookPerson.value == "" || bookDay.value == "" || time.value == "" || mess.value == "" || input.value == "" || input2.value == "") {
        error.innerHTML = "Please enter all the information";
    } else {
        error.classList.add("active");
        error.innerHTML = "You have successfully booked a table";
    }
});