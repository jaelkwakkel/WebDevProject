"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

//Disable the send button until connection is established.
document.getElementById("joinGroupButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    const li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    document.getElementById("joinGroupButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("joinGroupButton").addEventListener("click", function (event) {
    //TODO: Will be taken from logged in user, when logging in becomes a thing
    const user = document.getElementById("userInput").value;
    const group = document.getElementById("groupInput").value;
    connection.invoke("Join", user, group) //The join function in GameHub.cs
        .catch(err => {
                console.log(err);
            }
        );
    showGame();
    event.preventDefault();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function hideGame() {
    document.getElementById("lobby").classList.add('show');
    document.getElementById("lobby").classList.remove('hide');
    document.getElementById("game").classList.add('hide');
    document.getElementById("game").classList.remove('show');
}

function showGame() {
    document.getElementById("lobby").classList.add('hide');
    document.getElementById("lobby").classList.remove('show');
    document.getElementById("game").classList.add('show');
    document.getElementById("game").classList.remove('hide');
}