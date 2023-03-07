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

window.addEventListener("resize", () => {
    console.log("Resize!!");
    calculateCanvasSize();
});

//Add event listeners
canvas.addEventListener("mousemove", onMouseMove);
canvas.addEventListener("mouseleave", onMouseExit);

let mousePos = {x: -1, y: -1};

function onMouseMove(evt) {
    mousePos = getMousePos(evt);
    console.log("Mouse X : " + mousePos.x + ", Mouse Y : " + mousePos.y);
}

function onMouseExit(evt) {
    mousePos = {x: -1, y: -1};
    console.log("Mouse exited playing field");
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

// game loop
function gameLoop() {
    requestAnimationFrame(gameLoop);
    context.clearRect(0, 0, canvas.width, canvas.height);

    context.fillStyle = 'red';
    let gridCoordinate = gridCellToCoordinate(mousePosToGridCell(mousePos));
    context.fillRect(gridCoordinate.x, gridCoordinate.y, grid, grid);
}

// start the game
gameLoop();