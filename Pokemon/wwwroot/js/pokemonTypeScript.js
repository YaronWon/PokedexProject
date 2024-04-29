const colorsBack = {
	Normal: '#A8A77A',
	Fire: '#EE8130',
	Water: '#6390F0',
	Electric: '#F7D02C',
	Grass: '#7AC74C',
	Ice: '#96D9D6',
	Fighting: '#C22E28',
	Poison: '#A33EA1',
	Ground: '#E2BF65',
	Flying: '#A98FF3',
	Psychic: '#F95587',
	Bug: '#A6B91A',
	Rock: '#B6A136',
	Ghost: '#735797',
	Dragon: '#6F35FC',
	Dark: '#705746',
	Steel: '#B7B7CE',
	Fairy: '#D685AD',
}


var typeN = document.getElementById("typeName");
var list = document.querySelectorAll("#IndexCard");

//cards Background
var a = "radial-gradient(circle at 50% 0%,";
let FirstType = colorsBack[typeN.innerText];
let SecondType = 'white';

var BackHeaderColor = a + FirstType + ' 36%,' + SecondType + ' 36%)';
for (let i = 0; i < list.length; i++) {
    list[i].style.background = BackHeaderColor;
}


//BackgroundColor 
var as = "linear-gradient(to right,";
let sFirstType = colorsBack[typeN.innerText];
let sSecondType = 'white';

var BackBodyColor = as + sFirstType + ',' + sSecondType + ')';

var x = document.getElementsByTagName("BODY")[0];
x.style.background = BackBodyColor;


//hover effects on table

