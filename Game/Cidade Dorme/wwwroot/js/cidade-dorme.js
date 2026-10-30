console.log("cidade-dorme.js");
const form = document.getElementById("lobbyForm");

if (form) {

    const playerName = document.getElementById("playerName");
    const roomCode = document.getElementById("roomCode");
    const joinButton = document.getElementById("joinRoomButton");
    const createButton = document.getElementById("createRoomButton");
    const formMessage = document.getElementById("formMessage");
    const formMessageText = document.getElementById("formMessageText");


    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .withAutomaticReconnect()
        .build();

    function showMessage(message) {
        formMessageText.textContent = message;
        formMessage.classList.remove("hidden");
    }

    function clearMessage() {
        formMessageText.textContent = "";
        formMessage.classList.add("hidden");
    }

    function getPlayerName() {
        const name = playerName.value.trim();

        if (!name) {
            showMessage("Digite seu nome para continuar.");
            playerName.focus();
            return null;
        }

        clearMessage();
        return name;
    }

    roomCode.addEventListener("input", () => {
        roomCode.value = roomCode.value
            .toUpperCase()
            .replace(/[^A-Z0-9]/g, "")
            .slice(0, 6);

        clearMessage();
    });

    playerName.addEventListener("input", clearMessage);

    joinButton.addEventListener("click", () => {
        const name = getPlayerName();

        if (!name) return;

        const code = roomCode.value.trim();

        if (!code) {
            showMessage("Digite o código da sala em que deseja entrar.");
            roomCode.focus();
            return;
        }

        sessionStorage.setItem("playerName", name);
        sessionStorage.setItem("roomCode", code);

        window.location.href = "/room";
    });

    createButton.addEventListener("click", () => {
        const name = getPlayerName();

        if (!name) return;

        sessionStorage.setItem("playerName", name);

        window.location.href = "/room";
    });


    form.addEventListener("submit", event => {
        event.preventDefault();
    });
}
