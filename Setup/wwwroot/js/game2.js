"use strict";

//SETUP CONNECTION
const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

connection.start().then(function () {
    hideErrorMessage();
    $("#playButton").attr("disabled", false);
}).catch(function (err) {
    return console.error(err.toString());
});

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
    playbutton.click(startGame);
    $('#joinGameModal').modal({backdrop: 'static', keyboard: false});
};

connection.on('GameJoinError', (errorMessage) => {
    showErrorMessage(errorMessage);
});

connection.on('JoinedGroup', (key) => {
    $('#gameIdHeader').text(key);
    $('#joinGameModal').modal('hide');
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

const loadedImages = {};

function preloadImage(url) {
    let a = new Image();
    a.src = url;
    a.onload = () => loadedImages[url] = a;
}

//Preload tiles
// for (let item in buildingType) {
//     if (item === buildingType.grass) continue;
//     preloadImage("/Images/" + item + ".png");
// }

preloadImage("/Images/PlayingGrid.gif");
preloadImage("/Images/red.png");
preloadImage("/Images/blue.png");

let gameBoard = createGameBoard(10, 10);

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

function createGameBoard(columnCount, rowCount) {
    const map = [];
    for (let x = 0; x < columnCount; x++) {
        map[x] = []; // set up inner array
        for (let y = 0; y < rowCount; y++) {
            map[x][y] = 0;
        }
    }
    return map;
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
}

function onMouseDown() {
    placeBuilding();
}

function placeBuilding() {
    const gridPos = mousePosToGridCell(mousePos);
    console.log("Try placing on: (" + gridPos.x + "," + gridPos.y + ")")
    if (gridPos.x < 0 || gridPos.y < 0) {
        return;
    }
    gameBoard[gridPos.x][gridPos.y] = 1;
    connection.invoke('PlacedTile', gridPos.x)
        .catch(err => {
                console.log(err);
            }
        );
    updateGraphics();
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
            if (gameBoard[i][j] === 0) continue;
            if (gameBoard[i][j] === 1) {
                context.drawImage(loadedImages["/Images/red.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            } else {
                context.drawImage(loadedImages["/Images/blue.png"], gridCoordinate.x, gridCoordinate.y, grid, grid);
            }
        }
    }
}

//game loop
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

function chkLine(a, b, c, d) {
    // Check first cell non-zero and all cells match
    return ((a !== 0) && (a === b) && (a === c) && (a === d));
}

function chkWinner(bd) {
    // Check down
    for (let r = 0; r < 3; r++)
        for (let c = 0; c < 7; c++)
            if (chkLine(bd[r][c], bd[r + 1][c], bd[r + 2][c], bd[r + 3][c]))
                return bd[r][c];

    // Check right
    for (let r = 0; r < 6; r++)
        for (let c = 0; c < 4; c++)
            if (chkLine(bd[r][c], bd[r][c + 1], bd[r][c + 2], bd[r][c + 3]))
                return bd[r][c];

    // Check down-right
    for (let r = 0; r < 3; r++)
        for (let c = 0; c < 4; c++)
            if (chkLine(bd[r][c], bd[r + 1][c + 1], bd[r + 2][c + 2], bd[r + 3][c + 3]))
                return bd[r][c];

    // Check down-left
    for (let r = 3; r < 6; r++)
        for (let c = 0; c < 4; c++)
            if (chkLine(bd[r][c], bd[r - 1][c + 1], bd[r - 2][c + 2], bd[r - 3][c + 3]))
                return bd[r][c];

    return 0;
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
