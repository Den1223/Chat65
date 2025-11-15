const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

const messagesList = document.getElementById("messagesList");
const userList = document.getElementById("userList");

let currentUser = "";


connection.on("ReceiveMessage", (user, message) => {
    addMessageToChat({ user: user, text: message });
});



connection.on("UpdateUserList", (users) => {
    userList.innerHTML = "";
    users.forEach(user => {
        const li = document.createElement("li");
        li.classList.add("list-group-item");
        li.textContent = user;
        userList.appendChild(li);
    });
});



connection.on("LoadMessages", (messages) => {
    messagesList.innerHTML = "";
    messages.forEach(msg => addMessageToChat(msg));
    messagesList.scrollTop = messagesList.scrollHeight;
});



connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR error:", err));



document.getElementById("userInput").addEventListener("change", () => {
    const newUser = document.getElementById("userInput").value.trim();

    if (!newUser || newUser === currentUser) return;

    currentUser = newUser;

    connection.invoke("JoinChat", currentUser)
        .then(() => console.log("Joined chat as", currentUser))
        .catch(err => console.error("JoinChat error:", err));
});



document.getElementById("sendButton").addEventListener("click", sendMessage);

document.getElementById("messageInput").addEventListener("keydown", (e) => {
    if (e.key === "Enter") sendMessage();
});


function sendMessage() {
    const message = document.getElementById("messageInput").value.trim();

    if (!currentUser) {
        alert("Введіть ім'я!");
        return;
    }

    if (!message) return;

    connection.invoke("SendMessage", currentUser, message)
        .catch(err => console.error("SendMessage error:", err));

    document.getElementById("messageInput").value = "";
}



function addMessageToChat(msg) {
    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message");

    if (msg.user === currentUser) msgDiv.classList.add("user");
    else msgDiv.classList.add("other");

    msgDiv.innerHTML = `<strong>${msg.user}:</strong> ${msg.text}`;
    messagesList.appendChild(msgDiv);

    messagesList.scrollTop = messagesList.scrollHeight;
}
