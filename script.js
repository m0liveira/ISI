document.addEventListener("DOMContentLoaded", () => {
  console.log("Dashboard carregado");

  document.getElementById("get-all-games").addEventListener("click", getAllGames);
  document.getElementById("create-game").addEventListener("click", createGame);
  document.getElementById("get-game").addEventListener("click", getGame);
  document.getElementById("update-game").addEventListener("click", updateGame);
  document.getElementById("delete-game").addEventListener("click", deleteGame);
  document.getElementById("get-game-by-share-code").addEventListener("click", getGameByShareCode);
  document.getElementById("user-authentication").addEventListener("click", userAuthentication);
});

async function getAllGames() {
  clearData();
  try {
    const token = localStorage.getItem("accessToken");
    if (!token) {
      throw new Error("Nenhum token de acesso encontrado. Por favor, autentique-se primeiro.");
    }
    const response = await fetch("http://localhost:5007/api/Game", {
      method: "GET",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${token}`,
      },
    });
    if (!response.ok) {
      throw new Error(`Erro de rede: ${response.statusText}`);
    }
    const games = await response.json();
    console.log("Todos os jogos carregados:", games);
    displayGames(games);
  } catch (error) {
    console.error("Erro ao carregar jogos:", error);
  }
}

async function createGame() {
  // Implement create game logic
}

async function getGame() {
  // Implement get game logic
}

async function updateGame() {
  // Implement update game logic
}

async function deleteGame() {
  // Implement delete game logic
}

async function getGameByShareCode() {
  // Implement get game by share code logic
}

async function userAuthentication() {
  // Implement user authentication logic
}

function clearData() {
  const mainContent = document.getElementById("main-content");
  if (mainContent) {
    mainContent.innerHTML = "";
  }
}

function displayGames(games) {
  const mainContent = document.getElementById("main-content");
  if (!mainContent) {
    console.error("Elemento com o ID 'main-content' não encontrado.");
    return;
  }
  console.log("A exibir jogos:", games);

  const section = document.createElement("section");
  section.className = "dashboard-section";
  section.innerHTML = `<h2>Jogos</h2>`;

  const table = document.createElement("table");
  table.className = "dashboard-table";
  table.innerHTML = `
        <thead>
            <tr>
                <th>ID</th>
                <th>Host ID</th>
                <th>Data</th>
                <th>Endereço</th>
                <th>Capacidade</th>
                <th>Preço</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>
            ${games
              .map(
                (game) => `
                <tr>
                    <td>${game.id}</td>
                    <td>${game.host_id}</td>
                    <td>${game.date}</td>
                    <td>${game.address}</td>
                    <td>${game.capacity}</td>
                    <td>${game.price}</td>
                    <td>${game.status}</td>
                </tr>`
              )
              .join("")}
        </tbody>
    `;
  section.appendChild(table);
  mainContent.appendChild(section);
}
