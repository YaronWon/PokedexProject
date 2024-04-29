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


var typeButtons = document.querySelectorAll("#typeButton");
//BackDetailPic
var BackPic = document.querySelector(".BackDetailPic");

//background: radial-gradient(circle at 50% 10%, #EE8130 36%, #ffffff 36%);

var tempBack = " radial-gradient(circle at 50% 10%,";
var colorOne =  colorsBack[typeButtons[0].innerHTML];
var ColorTwo = typeButtons[1] == null ? colorsBack[typeButtons[1].innerHTML] : "#ffff";

BackPic.style.background = tempBack + colorOne + "36%," + ColorTwo + "36%)";




