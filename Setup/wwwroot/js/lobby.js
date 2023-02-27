"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

document.getElementById("sendButton").disabled = true;
// document.getElementById("joinButton").disabled = true;

connection.start().then(function () {
    // let name;
    //For debugging
    let name = "test";
    do {
        name = localStorage.getItem('name') || prompt('Input your name');
    } while (!name);
    localStorage.setItem('name', name);
    connection.invoke('AddUser', name).catch(err => {
        console.log(err)
    });

    const password = document.getElementById("pwInput").value;
    const id = document.getElementById("idInput").value;
    localStorage.setItem('gameId', id);

    if (id === "") {
        connection.invoke('CreateGame', password)
            .catch(err => {
                    console.log(err);
                }
            );
    } else {
        document.getElementById("gameIdHeader").innerHTML = id;
        connection.invoke('JoinGame', id, password)
            .catch(err => {
                    console.log(err);
                }
            );
    }

    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on('createdGame', (id) => {
    console.log("Created game with id: " + id);
    document.getElementById("idInput").value = id;
    document.getElementById("gameIdHeader").innerHTML = id;
});

connection.on('ErrorOnJoinGame', (message) => {
    console.log("Could not join game. Error: " + message);
    localStorage.setItem('joinError', message);
    location.href = "/game/"; //'/sometest.html?myid=5';
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const message = document.getElementById("messageInput").value;
    const id = document.getElementById("idInput").value;
    connection.invoke("SendMessage", message, id).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ReceiveMessage", function (message) {
    console.log("Received" + message)
    const li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${message}`;
});


// document.getElementById("joinButton").addEventListener("click", function (event) {
//     const password = document.getElementById("passwordInput").value;
//     const id = document.getElementById("idInput").value;
//     localStorage.setItem('gameId', id);
//     connection.invoke('JoinGame', id, password)
//         .catch(err => {
//                 console.log(err);
//             }
//         );
//     event.preventDefault();
// });