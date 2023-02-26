"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

document.getElementById("createButton").disabled = true;

connection.start().then(function () {

    let name = "test";
    do {
        name = localStorage.getItem('name') || prompt('Input your name');
    } while (!name);
    localStorage.setItem('name', name);
    connection.invoke('AddUser', name).catch(err => {
        console.log(err)
    });
    connection.invoke('UpdateAllGames').catch(err => {
        console.log(err)
    });

    document.getElementById("createButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("createButton").addEventListener("click", function (event) {
    const password = document.getElementById("passwordInput").value;
    connection.invoke("CreateGame", 2, password)
        .catch(err => {
                console.log(err);
            }
        );
    event.preventDefault();
});

connection.on('GameUpdate', (game) => {
    if (game.GameStarted) {
        if (location.href !== '/game/play') {
            location.href = '/game/play';
        }
    } else {
        if (location.href !== '/game/play#') {
            location.href = '/game/play#';
        }
    }
});