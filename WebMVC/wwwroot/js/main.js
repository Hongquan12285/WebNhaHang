// animation scroll
$(".scroll").click(function() {
    $("html, body").animate({
        scrollTop: 0
    }, 500);
});
// fixed navbar
$(window).scroll(function() {
    const sticky = $(".navbar"),
        scroll = $(window).scrollTop();

    if (scroll >= 50) {
        sticky.addClass("fixed");
    } else {
        sticky.removeClass("fixed");
    }
});

$(".nav-toggle").click(function() {
    $(".toggle-menu").slideToggle(500);
    $(".icon-dow").toggleClass("active");
});

// nhập thông tin contact
function loadCart() {
    let numberCart = localStorage.getItem("INFORCART") ? JSON.parse(localStorage.getItem("INFORCART")).length : 0;
    let couCart = document.querySelector(".cart span");
    couCart.innerHTML = numberCart;
}
loadCart();
//cap nhat icon login
const updateLogin = JSON.parse(localStorage.getItem("LOGIN"));
const textLogin = document.querySelector(".user .text-login");
const iconLogin = document.querySelector(".user .icon-login");
if (updateLogin) {
    textLogin.classList.add("active");
    iconLogin.classList.add("active");
}