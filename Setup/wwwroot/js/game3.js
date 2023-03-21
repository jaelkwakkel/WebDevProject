"use strict";

//SETUP CONNECTION
const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

function showErrorMessage(message) {
    $('#error-message').show();
    $('#error-message-content').text(message);
}

function hideErrorMessage() {
    $('#error-message').hide();
    $('#error-message-content').text("");
}

window.onload = function () {
    showErrorMessage("No connection");
    const playbutton = $('#playButton');
    playbutton.attr("disabled", true);
    $('#GamePlayErrorMessage').hide();
    playbutton.click(startGame);
    $('#joinGameModal').modal({backdrop: 'static', keyboard: false});

    connection.start().then(function () {
        hideErrorMessage();
        $("#playButton").attr("disabled", false);
    }).catch(function (err) {
        return console.error(err.toString());
    });
};

connection.on('GameJoinError', (errorMessage) => {
    showErrorMessage(errorMessage);
});

connection.on('JoinedGroup', (key) => {
    $('#gameIdHeader').text(key);
    $('#joinGameModal').modal('hide');
});

connection.on('GamePlayError', (message) => {
    console.log("New gameplay error: " + message);
    $('#GamePlayErrorMessage').text(message);
    $('#GamePlayErrorMessage').show(300);
});

connection.on('UpdateBoard', (board) => {
    updateGame(board);
    updateGraphics();
});

function startGame() {
    const key = $('#GameCode').val();
    if (!key || key.length < 6) {
        showErrorMessage("Key must be at least 6 characters");
        return;
    }
    if (key.length > 25) {
        showErrorMessage("Key can be no longer than 25 characters");
        return;
    }
    hideErrorMessage();
    connection.invoke("CreateOrJoin", key).catch(function (err) {
        return console.error(err.toString());
    });
}

//SETUP GAME CONSTANTS
const gridSize = 10;
const buildingType = {
    Grass: 0,
    Street: 1,
    House: 2,
    Farm: 3,
    Cinema: 4,
    EnergySmall: 5,
    EnergyLarge: 6,
    School: 7,
    Factory: 8
};

const loadedImages = {};

class GridCell {
    BuildingType;
    Owner;

    constructor() {
        this.BuildingType = buildingType.Grass;
    }
}

function preloadImage(url) {
    let a = new Image()
    a.src = url;
    a.onload = () => loadedImages[url] = a;
}

//Preload tiles
for (const item in buildingType) {
    if (item === "Grass") continue;
    preloadImage("/Images/" + item + ".png");
}
preloadImage("/Images/PlayingGrid.gif");

let gameBoard = createGameBoard(10, 10);
let currentlySelected = buildingType.Street;

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
        if (item === "Grass") {
            console.log("GRASSS");
            continue;
        }
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
    //console.log("Mouse exited playing field");
    //printBoard();
}

function onMouseDown() {
    placeBuilding();
}

function placeBuilding() {
    $('#GamePlayErrorMessage').hide(300);
    const gridPos = mousePosToGridCell(mousePos);
    console.log("Try placing on: (" + gridPos.x + "," + gridPos.y + ")")
    if (gridPos.x < 0 || gridPos.y < 0) {
        return;
    }

    //Only place tiles on empty places
    //TODO: M: Also check on server-side
    if (GetBuildingTypeFromNumber(gameBoard[gridPos.x][gridPos.y].BuildingType) !== "Grass") return;

    // connection.invoke("PlacedBuilding", "test123")
    //     .catch(err => {
    //             console.log(err);
    //         }
    //     );

    const values = {
        xPosition: gridPos.x.toString(),
        yPosition: gridPos.y.toString(),
        buildingType: currentlySelected.toString(),
    }

    //connection.invoke('ChangedBoard', JSON.stringify(gameBoard))
    //    .catch(err => {
    //        console.log(err);
    //    }
    //);

    console.log(JSON.stringify(values));
    connection.invoke("PlacedBuilding", JSON.stringify(values))
        .catch(err => {
                debugger;
                console.log(err);
            }
    );
    updateGraphics();
}

// function placeBuilding() {
//     const gridPos = mousePosToGridCell(mousePos);
//     console.log("Try placing on: (" + gridPos.x + "," + gridPos.y + ")")
//     if (gridPos.x < 0 || gridPos.y < 0) {
//         return;
//     }
//
//     if (gameBoard[gridPos.x][gridPos.y].buildingType !== buildingType.Grass) return;
//
//     gameBoard[gridPos.x][gridPos.y].buildingType = currentlySelected;
//     gameBoard[gridPos.x][gridPos.y].owner = 1;
     //connection.invoke('ChangedBoard', JSON.stringify(gameBoard))
     //    .catch(err => {
     //            console.log(err);
     //        }
     //    );
//     updateGraphics();
// }

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
            //Skip Grass tiles when drawing
            if (GetBuildingTypeFromNumber(gameBoard[i][j].BuildingType) === "Grass") continue;
            context.drawImage(loadedImages["/Images/" + GetBuildingTypeFromNumber(gameBoard[i][j].BuildingType) + ".png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
        }
    }
}

function updateGraphics() {
    context.clearRect(0, 0, canvas.width, canvas.height);

    drawBuildings();

    let gridCoordinate = gridCellToCoordinate(mousePosToGridCell(mousePos));
    //TODO: C: Make gif move
    context.drawImage(loadedImages["/Images/PlayingGrid.gif"], gridCoordinate.x, gridCoordinate.y, grid, grid);
}

function updateGame(board) {
    gameBoard = JSON.parse(board);
}

function GetBuildingTypeFromNumber(buildingTypeNumber) {
    switch (buildingTypeNumber) {
        case 0: return "Grass";
        case 1: return "Street";
        case 2: return "House";
        case 3: return "Farm";
        case 4: return "Cinema";
        case 5: return "EnergySmall";
        case 6: return "EnergyLarge";
        case 7: return "School";
        case 8: return "Factory";
        default: return "Helemaal mis------------";
    };
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
