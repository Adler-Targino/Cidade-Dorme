console.log("room.js");
const loadingState = document.getElementById("loadingState");
const loadingTitle = document.getElementById("loadingTitle");
const loadingDescription = document.getElementById("loadingDescription");

const roomState = document.getElementById("roomState");
const errorState = document.getElementById("errorState");
const errorMessage = document.getElementById("errorMessage");

const roomCode = document.getElementById("roomCode");
const playerList = document.getElementById("playerList");
const playerCount = document.getElementById("playerCount");

const leaveRoomButton = document.getElementById("leaveRoomButton");
const retryButton = document.getElementById("retryButton");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .withAutomaticReconnect()
    .build();

let currentRoomCode = null;
let isLoadingRoom = false;

function showLoading(title, description) {
    loadingTitle.textContent = title;
    loadingDescription.textContent = description;

    loadingState.classList.remove("hidden");
    loadingState.classList.add("hidden");
    errorState.classList.add("hidden");
}

function showRoom() {
    loadingState.classList.add("hidden");
    roomState.classList.remove("hidden");
    errorState.classList.add("hidden");
}

function showError(message) {
    loadingState.classList.add("hidden");
    roomState.classList.add("hidden");
    errorState.classList.remove("hidden");

    errorMessage.textContent = message;
}

function renderPlayers(players) {
    const playerList = document.getElementById("playerList");
    const playerCount = document.getElementById("playerCount");

    playerList.innerHTML = "";

    players.forEach(player => {
        const item = document.createElement("li");
        item.className = "flex items-center gap-3 rounded-box bg-base-200 p-3";

        // Connection ID oculto
        const connectionId = document.createElement("input");
        connectionId.type = "hidden";
        connectionId.value = player.connectionId;
        connectionId.className = "player-connection-id";

        const avatar = document.createElement("div");
        avatar.className = "avatar placeholder";

        const avatarContent = document.createElement("div");
        avatarContent.className =
            "flex h-10 w-10 items-center justify-center rounded-full bg-primary text-primary-content";

        avatarContent.textContent = player.name.charAt(0).toUpperCase();

        const name = document.createElement("span");
        name.className = "font-medium";
        name.textContent = player.name;

        item.appendChild(connectionId);
        item.appendChild(avatar);
        item.appendChild(name);

        playerList.appendChild(item);
    });

    playerCount.textContent = players.length;
}

connection.on("PlayerListUpdated", players => {
    renderPlayers(players);
});

async function initializeRoom() {
    console.log("initialize room...");
    if (isLoadingRoom) return;

    isLoadingRoom = true;

    const name = sessionStorage.getItem("playerName");
    let code = sessionStorage.getItem("roomCode");

    if (!name) {
        window.location.replace("/");
        return;
    }

    if (!code) { //Cria uma sala
        showLoading(
            "Criando sala...",
            "Estamos preparando a sala para você."
        );

        try {
            if (connection.state === signalR.HubConnectionState.Disconnected) {
                console.log("Connection start");
                await connection.start();
            }

            code = await connection.invoke("CreateRoom", name);

            currentRoomCode = code;
            roomCode.textContent = code;

            console.log("Show room");
            showRoom();
        } catch (error) {
            console.error("Erro ao criar sala:", error);
            showError("Não foi possível criar a sala. Verifique sua conexão e tente novamente.");
        } finally {
            isLoadingRoom = false;
        }
    } else { // Entra em uma sala
        try {
            if (connection.state === signalR.HubConnectionState.Disconnected) {
                console.log("Connection start");
                await connection.start();
            }

            code = await connection.invoke("JoinRoom", code, name);

            currentRoomCode = code;
            roomCode.textContent = code;

            console.log("Show room");
            showRoom();
        } catch (error) {
            console.error("Erro ao entrar na sala:", error);
            showError("Não foi possível se juntar a sala. Verifique sua conexão e tente novamente.");
        } finally {
            isLoadingRoom = false;
        }
    }

}

leaveRoomButton.addEventListener("click", async () => {
    try {
        await connection.stop();
    } finally {
        sessionStorage.removeItem("playerName");
        window.location.href = "/";
    }
});

retryButton.addEventListener("click", initializeRoom);

initializeRoom();
