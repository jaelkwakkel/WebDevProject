"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    console.log("Received" + message)
    const li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${message}`;
});

connection.start().then(function () {
    const password = document.getElementById("pwInput").value;
    const id = document.getElementById("idInput").value;
    localStorage.setItem('gameId', id);
    connection.invoke('JoinGame', id, password)
        .catch(err => {
                console.log(err);
            }
        );
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message, localStorage.getItem('gameId')).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
