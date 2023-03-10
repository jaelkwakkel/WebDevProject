// "use strict";
//
// let errorMessage = localStorage.getItem('joinError')
//
// if (errorMessage.length > 0) {
//     //TaODO: Prevent possible xss/injection
//     document.getElementById("joinErrorMessage").innerHTML = errorMessage;
//     localStorage.setItem('joinError', "");
//     document.getElementById('joinGameModal').show();
// }

"use strict";

//GAME_CONSTANTS
const gridSize = 10;

const buildingType = Object.freeze({
    grass: Symbol("grass"),
    street: Symbol("street"),
    house: Symbol("house"),
    farm: Symbol("farm"),
    cinema: Symbol("cinema"),
    energy_small: Symbol("energy_small"),
    energy_large: Symbol("energy_large"),
    school: Symbol("school"),
    factory: Symbol("factory")
});

const loadedImages = {};

class GridCell {
    buildingType;
    owner;

    constructor() {
        this.buildingType = buildingType.grass;
    }

    placeBuilding(owner, buildingType) {
        this.buildingType = buildingType;
        this.owner = owner;
        updateGraphics();
    }
}

class BuildingButton extends HTMLElement {
    shadowRoot;

    constructor(name) {
        super();
        this.shadowRoot = this.attachShadow({mode: 'open'});
        const button = document.createElement('button');
        button.setAttribute('class', 'list-group-item');
        button.setAttribute('class', 'list-group-item-action');
        button.innerHTML = name;
        this.shadowRoot.appendChild(button);
    }
}

function preloadImage(url) {
    let a = new Image()
    a.src = url;
    a.onload = () => loadedImages[url] = a;
}

preloadImage("/Images/House.png");
preloadImage("/Images/PlayingGrid.gif");

customElements.define('building-button', BuildingButton);

let gameBoard = createGameBoard(10, 10);
let currentlySelected = buildingType.street;

function selectedBuilding(event) {
    //Set selected button as active, others as inactive
    //TODO: S: Change to radio type
    let elems = document.getElementsByClassName("active");
    for (let i = 0; i < elems.length; i++) {
        elems[i].classList.remove("active");
    }
    const source = event.target || event.srcElement;
    source.classList.add("active");

    currentlySelected = buildingType[source.getAttribute('buildingType')];
    console.log(currentlySelected);
}

function createButtons() {
    const gameList = document.getElementById("buildingSelectList");
    for (let item in buildingType) {
        const button = document.createElement('button');
        button.setAttribute('class', 'list-group-item list-group-item-action align-middle');
        button.addEventListener("click", selectedBuilding);
        button.setAttribute('buildingType', item);
        button.innerHTML = item;
        gameList.appendChild(button);
    }
    gameList.children.item(0).classList.add("active");
}

createButtons();

function createGameBoard(columnCount, rowCount) {
    const map = [];
    for (let x = 0; x < columnCount; x++) {
        map[x] = []; // set up inner array
        for (let y = 0; y < rowCount; y++) {
            addCell(map, x, y);
        }
    }
    return map;
}

function addCell(map, x, y) {
    map[x][y] = new GridCell(); // create a new object on x and y
}

//Get and setup canvas
const canvas = document.getElementById('game');
const context = canvas.getContext('2d');
//Keep square
let grid;
calculateCanvasSize()

function calculateCanvasSize() {
    const heightRatio = 1;
    // canvas.width = canvas.parentElement.width;
    canvas.height = canvas.width * heightRatio;
    grid = canvas.width / gridSize;
}

//Add event listeners
window.addEventListener("resize", () => {
    console.log("Resize!!");
    calculateCanvasSize();
    updateGraphics();
});
canvas.addEventListener("mousemove", onMouseMove);
canvas.addEventListener("mouseleave", onMouseExit);
canvas.addEventListener("mousedown", onMouseDown)

let mousePos = {x: -1, y: -1};

function onMouseMove(evt) {
    mousePos = getMousePos(evt);
    updateGraphics();
    // console.log("Mouse X : " + mousePos.x + ", Mouse Y : " + mousePos.y);
}

function onMouseExit(evt) {
    mousePos = {x: -1, y: -1};
    updateGraphics();
    console.log("Mouse exited playing field");
    printBoard();
}

function onMouseDown() {
    const gridPos = mousePosToGridCell(mousePos);
    console.log("Try placing on: (" + gridPos.x + "," + gridPos.y + ")")
    if (gridPos.x < 0 || gridPos.y < 0) {
        return;
    }
    gameBoard[gridPos.x][gridPos.y].placeBuilding(1, currentlySelected);
}

const clamp = (number, min, max) =>
    Math.max(min, Math.min(number, max));

//From: https://stackoverflow.com/questions/17130395/real-mouse-position-in-canvas
function getMousePos(evt) {
    const rect = canvas.getBoundingClientRect(), // abs. size of element
        scaleX = canvas.width / rect.width,    // relationship bitmap vs. element for x
        scaleY = canvas.height / rect.height;  // relationship bitmap vs. element for y

    return {
        x: (evt.clientX - rect.left) * scaleX,   // scale mouse coordinates after they have
        y: (evt.clientY - rect.top) * scaleY     // been adjusted to be relative to element
    }
}

function mousePosToGridCell({x, y}) {
    if (x < 0 || y < 0) {
        return {
            x: -1,
            y: -1
        }
    }
    return {
        //Add 0.5 to get cell-center
        x: clamp(Math.round((x / grid) + 0.5), 1, gridSize) - 1,
        y: clamp(Math.round((y / grid) + 0.5), 1, gridSize) - 1
    };
}

function gridCellToCoordinate({x, y}) {
    return {
        x: x * grid,
        y: y * grid
    };
}

function drawBuildings() {
    //Loop trough game board
    for (let i = 0; i < gameBoard.length; i++) {
        for (let j = 0; j < gameBoard[i].length; j++) {
            let gridCoordinate = gridCellToCoordinate({x: i, y: j});

            context.drawImage(loadedImages["/Images/" + gameBoard[i][j].buildingType + ".png"], gridCoordinate.x, gridCoordinate.y, grid, grid);

            // switch (gameBoard[i][j].buildingType) {
            //     case buildingType.grass:
            //         break;
            //     case buildingType.street:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            //     case buildingType.house:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            //     case buildingType.farm:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            //     case buildingType.cinema:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            //     case buildingType.energy_small:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            //     case buildingType.energy_large:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            //     case buildingType.school:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            //     case buildingType.factory:
            //         context.drawImage(loadedImages["/Images/House.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            //         break;
            // }
        }
    }
}

// game loop
function updateGraphics() {
    context.clearRect(0, 0, canvas.width, canvas.height);

    drawBuildings();

    let gridCoordinate = gridCellToCoordinate(mousePosToGridCell(mousePos));
    //TODO: C: Make gif move
    context.drawImage(loadedImages["/Images/PlayingGrid.gif"], gridCoordinate.x, gridCoordinate.y, grid, grid);
}

//DEBUG-FUNCTIONS
function printBoard() {
    let board = "";
    gameBoard.forEach(x => {
        x.forEach(y => {
            board += "[" + y.buildingType.toString() + "]";
        })
        board += "\n";
    })
    console.log(board);
}
