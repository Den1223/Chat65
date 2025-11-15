const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

const messagesList = document.getElementById("messagesList");
const userList = document.getElementById("userList");

let currentUser = "";

// --- Отримання повідомлень ---
connection.on("ReceiveMessage", (user, message) => {
    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message");

    if (user === currentUser) {
        msgDiv.classList.add("user");
    } else {
        msgDiv.classList.add("other");
    }

    msgDiv.innerHTML = `<strong>${user}:</strong> ${message}`;

    messagesList.appendChild(msgDiv);
    messagesList.scrollTop = messagesList.scrollHeight;
});

// --- Оновлення списку користувачів ---
connection.on("UpdateUserList", (users) => {
    userList.innerHTML = "";
    users.forEach(user => {
        const li = document.createElement("li");
        li.classList.add("list-group-item");
        li.textContent = user;
        userList.appendChild(li);
    });
});

// --- Підключення ---
connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR error: " + err));

// --- Введення імені ---
document.getElementById("userInput").addEventListener("change", () => {
    const user = document.getElementById("userInput").value.trim();
    if (user && user !== currentUser) {

        currentUser = user;

        connection.invoke("JoinChat", user)
            .then(() => console.log("JoinChat OK:", user))
            .catch(err => console.error(err.toString()));
    }
});

// --- Надсилання повідомлення ---
document.getElementById("sendButton").addEventListener("click", () => {
    const message = document.getElementById("messageInput").value.trim();

    if (!currentUser) {
        alert("Введіть ім'я!");
        return;
    }

    if (message.length > 0) {
        connection.invoke("SendMessage", currentUser, message)
            .catch(err => console.error(err.toString()));

        document.getElementById("messageInput").value = "";
    }
});
