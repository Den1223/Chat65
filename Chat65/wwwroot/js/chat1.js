
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

const messagesList = document.getElementById("messagesList");
const userList = document.getElementById("userList");

// Отримання повідомлень
connection.on("ReceiveMessage", (user, message) => {
    const li = document.createElement("li");
    li.classList.add("message");
    li.textContent = message;

    const currentUser = document.getElementById("userInput").value;
    if (user === currentUser) {
        li.classList.add("user");
    } else {
        li.classList.add("other");
    }

    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight; // прокрутка вниз
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
