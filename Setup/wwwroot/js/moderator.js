"use strict";

//SETUP CONNECTION
const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

window.onload = function () {

    $('#GamePlayErrorMessage').hide();
    $('#GamePlayMessage').hide();

    $('#removeAll').click(removeAllFinishedGames);
    connection.start().then(function () {
        connection.invoke("GetActiveGames")
            .catch(function (err) {
                return console.error(err.toString());
            });
    }).catch(function (err) {
        return console.error(err.toString());
    });
};

connection.on('Abort', (errorMessage) => {
    console.log(errorMessage);
    //window.location.href = "/";
});

connection.on('Message', (Message) => {
    showMessage(Message);
});

connection.on('ErrorMessage', (Message) => {
    showErrorMessage(Message);
});

connection.on('UpdateGameList', (users) => {
    const parsedGames = JSON.parse(users);

    console.log("UpdateGameList----------!!!");
    console.log(parsedGames);

    const gameList = document.getElementById("gameList");

    //Clear list
    while (gameList.firstChild) {
        gameList.lastChild.remove();
    }

    for (let item in parsedGames) {
        const button = document.createElement('button');
        button.innerHTML = parsedGames[item].Item1 + " - finished: " + parsedGames[item].Item2;
        button.setAttribute('class', 'btn btn-secondary');
        button.setAttribute('key', parsedGames[item].Item1);
        button.addEventListener('click', removeGame);
        gameList.appendChild(button);
    }
});

function removeGame() {
    const source = event.target || event.srcElement;
    const key = source.getAttribute('key');

    connection.invoke("RemoveGame", key)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function removeAllFinishedGames() {
    connection.invoke("RemoveAllFinishedGames")
        .catch(function (err) {
            return console.error(err.toString());
        });
}


function showMessage(message) {
    console.log(message);
    $('#GamePlayMessage').text(message);
    $('#GamePlayMessage').show();
}

function showErrorMessage(message) {
    console.log(message);
    $('#GamePlayErrorMessage').text(message);
    $('#GamePlayErrorMessage').show();
}