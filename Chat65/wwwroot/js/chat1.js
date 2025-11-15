
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

const messagesList = document.getElementById("messagesList");
const userList = document.getElementById("userList");


connection.on("ReceiveMessage", (user, message) => {
    const container = document.getElementById("messagesList");

    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message");

    const currentUser = document.getElementById("userInput").value;
    if (user === currentUser) {
        msgDiv.classList.add("user");
    } else {
        msgDiv.classList.add("other");
    }

    msgDiv.innerHTML = `<strong>${user}:</strong> ${message}`;

    container.appendChild(msgDiv);
    container.scrollTop = container.scrollHeight;
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


connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR error: " + err));

document.getElementById("userInput").addEventListener("change", () => {
    const user = document.getElementById("userInput").value.trim();
    if (user) {
        connection.invoke("JoinChat", user)
            .catch(err => console.error(err.toString()));
    }
});
