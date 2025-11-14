
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

// Оновлення списку онлайн користувачів
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

document.getElementById("sendButton").addEventListener("click", () => {
    const user = document.getElementById("userInput").value.trim();
    const message = document.getElementById("messageInput").value.trim();

    if (!user) {
        alert("Введіть ім'я!");
        return;
    }

    if (message) {
        connection.invoke("SendMessage", user, message)
            .catch(err => console.error(err.toString()));
        document.getElementById("messageInput").value = "";
    }

    
    connection.invoke("JoinChat", user)
        .catch(err => console.error(err.toString()));
});
