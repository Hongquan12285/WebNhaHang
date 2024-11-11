const item1 = document.querySelector("#item-1");
const item2 = document.querySelector("#item-2");
const item3 = document.querySelector("#item-3");
const item4 = document.querySelector("#item-4");
const item11 = document.querySelector("#item11");
const item12 = document.querySelector("#item12");
const item13 = document.querySelector("#item13");
const item14 = document.querySelector("#item14");
const products = [{
        nameFood: "Greek Salad",
        price: "$25.50",
        image: "img/product/food-1.png",
        title: "Tomatoes, green bell pepper, sliced cucumber onion, olives, and feta cheese.",
    },
    {
        nameFood: "Lasagne",
        price: "$40.00",
        image: "img/product/food-2.png",
        title: "Vegetables, cheeses, ground meats, tomato sauce, seasonings and spices",
    },
    {
        nameFood: "Butternut Pumpkin",
        price: "$10.00",
        image: "img/product/food-3.png",
        title: "Typesetting industry lorem Lorem Ipsum is simply dummy text of the priand.",
    },
];
const productsFood = [{
        nameFood: "Tokusen Wagyu",
        price: "$39.50",
        image: "img/product/food-4.png",
        title: "Vegetables, cheeses, ground meats, tomato sauce, seasonings and spices.",
    },
    {
        nameFood: "Olivas Rellenas",
        price: "$25.00",
        image: "img/product/food-5.png",
        title: "Avocados with crab meat, red onion, crab salad stuffed red bell pepper and green bell pepper.",
    },
    {
        nameFood: "Opu Fish",
        price: "$49.00",
        image: "img/product/food-6.png",
        title: "Vegetables, cheeses, ground meats, tomato sauce, seasonings and spices.",
    },
];

function renderCard(products, productsFood, item1, item11) {
    products.forEach((product) => {
        item1.innerHTML += `  
        <div class="content ">
            <div class="food d-flex">
                <div class="images">
                    <img src="${product.image}" alt="" />
                </div>
                <div class="des">
                    <div class="text">
                        <h5>${product.nameFood}</h5>
                        <h6>${product.price}</h6>
                    </div>
                    <p>${product.title}</p>
                </div>
            </div>
    </div>                 
        `;
    });
    productsFood.forEach((product) => {
        item11.innerHTML += `
        <div class="content ">
            <div class="food d-flex">
                <div class="images">
                    <img src="${product.image}" alt="" />
                </div>
                <div class="des">
                    <div class="text">
                        <h5>${product.nameFood}</h5>
                        <h6>${product.price}</h6>
                    </div>
                    <p>${product.title}</p>
                </div>
            </div>
        </div>          
    `;
    });
}
renderCard(products, productsFood, item1, item11);
renderCard(productsFood, products, item2, item12);
renderCard(products, productsFood, item3, item13);
renderCard(productsFood, products, item4, item14);
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