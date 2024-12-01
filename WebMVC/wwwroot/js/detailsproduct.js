let arrayProduct;
if (JSON.parse(localStorage.getItem("INFORCART"))) {
    arrayProduct = JSON.parse(localStorage.getItem("INFORCART"));
} else {
    arrayProduct = [];
}
const product = JSON.parse(localStorage.getItem("PRODUCT"));
console.log(product);
const details = document.querySelector(".info-product");

function renderDetailProduct() {
    details.innerHTML = `<h3>${product.nameFood || product.nameWine}</h3>
        <ul class="start">
        <li><i class="fa-solid fa-star"></i></li>
        <li><i class="fa-solid fa-star"></i></li>
        <li><i class="fa-solid fa-star"></i></li>
        <li><i class="fa-solid fa-star"></i></li>
        <li><i class="fa-solid fa-star"></i></li>
    </ul>
            <p class="price-product"><span>${product.price}</span></p>
            <p class="text">${product.title}</p>`;
}

const imgHtml = document.querySelector(".product .list-image .image");
const imgSlideFirst = document.querySelector("li.ava");
imgHtml.innerHTML = ` <img src="${product.image}" alt="" />`;
imgSlideFirst.innerHTML = ` <img src="${product.image}" alt="" />`;

renderDetailProduct();
$(function() {
    $(".dropdown-type").click(function() {
        $(".dropdown-select").slideToggle("500");
        $(".dropdown-type i ").toggleClass("active");
    });
    $(".dropdown-select li").click(function() {
        $(".dropdown-select li").removeClass("active");
        $(this).addClass("active");
        $chosse = $(this).text();
        $(".dropdown-type span").text($chosse);
        $(".dropdown-select").slideUp("500");
        $(this).siblings().removeClass("active");
    });
    $(".small-image ul li").click(function() {
        $newSrc = $(this).find("img").attr("src");
        $image = $(".product .list-image .image img").attr("src", $newSrc);
    });
    $(".dropdown-select .normal").click(function() {
        $priceNormal = $(".price-product span").text(product.priceNormal);
        $(".dropdown-type i").removeClass("active");
        $(".dropdown .attention").text("");
    });
    $(".dropdown-select .vip").click(function() {
        $priceVip = $(".price-product span").text(product.priceVip);
        $(".dropdown-type i").removeClass("active");
        $(".dropdown .attention").text("");
    });
    $(".dropdown .btn2").click(function() {
        if (!$(".dropdown-select li").hasClass("active")) {
            $(".dropdown .attention").text("You have not selected a category.");
            return false;
        } else {
            $(".dropdown .attention").text("");
            $productImage = $(".product .list-image .small-image ul .ava img").attr("src");
            $(".popup .popup-product .image img").attr("src", $productImage);
            $(".popup .popup-product .popup-item h6 i").text(product.name);
            let chooseSelect = $(".dropdown-select .active").text();
            $(".popup .popup-product .popup-item .select i").text(chooseSelect);
            let choosePrice = $(".price-product span").text();
            $(".popup .popup-product .popup-item .price").text(choosePrice);
            $(".popup .popup-product .popup-item .quantity-product i").text("1");
            $(".popup").addClass("active");
            setTimeout(() => {
                $(".popup").removeClass("active");
            }, 2000);
            let infoProduct = {
                nameItem: product.nameFood || product.nameWine,
                image: product.image,
                select: chooseSelect,
                price: choosePrice,
                priceInner: product.priceInner,
                quantity: 1,
            };
            arrayProduct.push(infoProduct);
            console.log(arrayProduct);
            localStorage.setItem("INFORCART", JSON.stringify(arrayProduct));
            $CouCart = $(".cart span").text();
            $(".cart span").text(parseInt($CouCart) + 1);
        }
    });
});