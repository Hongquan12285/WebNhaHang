// show modal
const modal = document.getElementById("videoModal");
const btnModal = document.getElementById("openModal");
const span = document.getElementsByClassName("close")[0];
btnModal.onclick = function() {
    modal.style.display = "block";
};
span.onclick = function() {
    modal.style.display = "none";
};
window.onclick = function(event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
};

document.addEventListener("DOMContentLoaded", function () {
    const scrollers = document.querySelectorAll(".numscroller");

    scrollers.forEach(scroller => {
        const max = parseInt(scroller.getAttribute("data-max"));
        const increment = parseInt(scroller.getAttribute("data-increment"));
        const delay = parseInt(scroller.getAttribute("data-delay"));

        let current = 0;
        const interval = setInterval(() => {
            current += increment;
            if (current >= max) {
                current = max;
                clearInterval(interval);
            }
            scroller.textContent = current;
        }, delay);
    });
});