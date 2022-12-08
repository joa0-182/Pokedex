// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function filter(type) {
    let cards, i;
    let count = 0;
    cards = document.getElementsByClassName("card");
    buttons =  document.getElementsByClassName("btn-filter");
    for (i = 0; i < cards.length; i++) {
        if (cards[i].classList.contains(type) || type === "all") {
            cards[i].parentElement.style.display = 'block';
            count += 1;
        }else {
            cards[i].parentElement.style.display = 'none';
        }
    };
    for (i = 0; i < buttons.length; i++) {
        if (buttons[i].id == `btn-${type}`) {
            buttons[i].classList.remove("btn-sm");
            buttons[i].classList.add("btn-lg");
        }
        else {
            buttons[i].classList.remove("btn-lg");
            buttons[i].classList.add("btn-sm");
        }
    };
    if (type === "all") {
        document.getElementById("btn-all").classList.remove("btn-sm");
        document.getElementById("btn-all").classList.add("btn-lg");
    };
    if (count == 0){
        document.getElementById("zeroPokemon").style.display = 'block';
    }
    else {
        document.getElementById("zeroPokemon").style.display = 'none';
    }
}