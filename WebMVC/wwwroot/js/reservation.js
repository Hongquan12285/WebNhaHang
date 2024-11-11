const button = document.querySelector(".group-info .btn2");
const error = document.querySelector(".error");
const pop = document.querySelector(".attention-pop");
button.addEventListener("click", function() {
    const bookPerson = document.querySelector(" .group1").value;
    const bookDay = document.querySelector("#datepicker").value;
    const time = document.querySelector(" .time").value;
    const input = document.querySelector("#name").value;
    const input2 = document.querySelector("#email").value;
    if (input == "") {
        error.innerHTML = "Please enter your name";
    } else if (input2 == "") {
        error.innerHTML = "Please enter your email";
    } else if (bookDay == "") {
        error.innerHTML = "Please select a date to reserve your table";
    } else {
        error.innerHTML = "";
        pop.classList.add("active");
        setTimeout(() => {
            pop.classList.remove("active");
        }, 2000);
        let info = [{
            name: input,
            email: input2,
            bookPerson: bookPerson,
            bookDay: bookDay,
            time: time,
        }, ];
        localStorage.setItem("INFORBOOKNG", JSON.stringify(info));
    }
});
$(function() {
    $("#datepicker").datepicker();
});