// Ajuste esta URL se a API estiver em outra porta/host
const API_BASE_URL = window.APP_CONFIG.API_BASE_URL;

let authToken = null;
let currentUser = null;

// cache pra stats
let cachedFriends = [];
let cachedGames = [];
let cachedActiveLoans = [];
let editingFriendId = null;
const $ = (id) => document.getElementById(id);
let editingLoanId = null;
let friendsPage = 1;
let gamesPage = 1;
let loansPage = 1;

const PAGE_SIZE = 10; 


function updateStats() {
    $("statFriends").textContent = cachedFriends.length;
    $("statGames").textContent = cachedGames.length;
    $("statLoans").textContent = cachedActiveLoans.length;
}

function setLoggedOutState() {
    authToken = null;
    currentUser = null;
    localStorage.removeItem("gamesloan_token");
    localStorage.removeItem("gamesloan_user");

    $("loginView").classList.remove("hidden");
    $("mainView").classList.add("hidden");
    $("userInfo").textContent = "Não autenticado";
}

function setLoggedInState(username, token) {
    authToken = token;
    currentUser = username;
    localStorage.setItem("gamesloan_token", token);
    localStorage.setItem("gamesloan_user", username);

    $("loginView").classList.add("hidden");
    $("mainView").classList.remove("hidden");
    $("userInfo").textContent = `Autenticado como ${username}`;

    loadFriends();
    loadGames();
    loadLoans();
    loadLoanSelects();
}

async function apiRequest(path, options = {}) {
    const headers = options.headers || {};
    headers["Content-Type"] = "application/json";

    if (authToken) {
        headers["Authorization"] = `Bearer ${authToken}`;
    }

    const resp = await fetch(`${API_BASE_URL}${path}`, {
        ...options,
        headers
    });

    const isJson = resp.headers.get("Content-Type")?.includes("application/json");
    const body = isJson ? await resp.json().catch(() => null) : null;

    if (!resp.ok) {
        const message =
            body?.error ||
            body?.message ||
            (body && typeof body === "string" ? body : `Erro HTTP ${resp.status}`);
        throw new Error(message);
    }

    return body;
}

// LOGIN
$("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    $("loginError").textContent = "";

    const username = $("loginUsername").value.trim();
    const password = $("loginPassword").value;

    if (!username || !password) {
        $("loginError").textContent = "Preencha usuário e senha.";
        return;
    }

    try {
        const res = await apiRequest("/Auth/login", {
            method: "POST",
            body: JSON.stringify({ username, password })
        });

        if (!res?.token) {
            throw new Error("Resposta de login inválida.");
        }

        setLoggedInState(res.username, res.token);
    } catch (err) {
        $("loginError").textContent = err.message;
    }
});
$("registerForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    $("registerMessage").textContent = "";
    $("registerError").textContent = "";

    const username = $("registerUsername").value.trim();
    const password = $("registerPassword").value;
    const passwordConfirm = $("registerPasswordConfirm").value;

    if (!username || !password) {
        $("registerError").textContent = "Usuário e senha são obrigatórios.";
        return;
    }

    if (password !== passwordConfirm) {
        $("registerError").textContent = "As senhas não conferem.";
        return;
    }

    try {
        await apiRequest("/Auth/register", {
            method: "POST",
            body: JSON.stringify({
                username,
                password
            })
        });

        $("registerMessage").textContent = "Usuário cadastrado com sucesso. Agora faça login.";
        $("registerUsername").value = "";
        $("registerPassword").value = "";
        $("registerPasswordConfirm").value = "";

        // opcional: já preenche o login com o usuário criado
        $("loginUsername").value = username;
    } catch (err) {
        $("registerError").textContent = err.message;
    }
});
// LOGOUT
$("logoutBtn").addEventListener("click", () => {
    setLoggedOutState();
});

// TABS
document.querySelectorAll(".tab[data-tab]").forEach((btn) => {
    btn.addEventListener("click", () => {
        document.querySelectorAll(".tab").forEach((b) => b.classList.remove("active"));
        btn.classList.add("active");

        const target = btn.getAttribute("data-tab");
        document.querySelectorAll(".tab-content").forEach((sec) => {
            sec.classList.toggle("hidden", sec.id !== target);
        });

        if (target === "friendsTab") loadFriends();
        if (target === "gamesTab") loadGames();
        if (target === "loansTab") {
            loadLoans();
            loadLoanSelects();
        }
    });
});

// -------- AMIGOS --------

function validarFriendForm() {
    const nome = $("friendName").value.trim();
    const email = $("friendEmail").value.trim();
    const phone = $("friendPhone").value.trim();

    if (!nome || !email || !phone) {
        throw new Error("Preencha todos os campos do amigo.");
    }

    // validação simples de e-mail
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        throw new Error("E-mail inválido.");
    }

    // validação simples de telefone
    const phoneRegex = /^\+?[0-9\s\-().]{8,20}$/;
    if (!phoneRegex.test(phone)) {
        throw new Error("Telefone inválido.");
    }

    return { nome, email, phone };
}

async function loadFriends() {
    $("friendsError").textContent = "";
    const tbody = $("friendsTable").querySelector("tbody");
    tbody.innerHTML = "<tr><td colspan='4'>Carregando...</td></tr>";

    try {
        const friends = await apiRequest("/Friends");
        cachedFriends = friends || [];
        friendsPage = 1;
        updateStats();
        renderFriendsTable();
    } catch (err) {
        $("friendsError").textContent = err.message;
        tbody.innerHTML = "";
        $("friendsPagination").innerHTML = "";
    }
}
function renderFriendsTable() {
    const tbody = $("friendsTable").querySelector("tbody");

    if (!cachedFriends || cachedFriends.length === 0) {
        tbody.innerHTML = "<tr><td colspan='4'>Nenhum amigo encontrado.</td></tr>";
        $("friendsPagination").innerHTML = "";
        return;
    }

    const total = cachedFriends.length;
    const totalPages = Math.ceil(total / PAGE_SIZE);

    // se página atual estourar, volta pra última
    if (friendsPage > totalPages) friendsPage = totalPages;

    const start = (friendsPage - 1) * PAGE_SIZE;
    const pageItems = cachedFriends.slice(start, start + PAGE_SIZE);

    tbody.innerHTML = "";
    pageItems.forEach((f) => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${f.name}</td>
            <td>${f.email ?? ""}</td>
            <td>${f.phone ?? ""}</td>
            <td>
                <button class="small-btn edit-friend" data-id="${f.id}">Editar</button>
                <button class="small-btn danger delete-friend" data-id="${f.id}">Excluir</button>
            </td>
        `;
        tbody.appendChild(tr);
    });

    // liga eventos editar/excluir
    tbody.querySelectorAll(".edit-friend").forEach((btn) => {
        btn.addEventListener("click", () => {
            const id = parseInt(btn.getAttribute("data-id"), 10);
            startEditFriend(id);
        });
    });

    tbody.querySelectorAll(".delete-friend").forEach((btn) => {
        btn.addEventListener("click", async () => {
            const id = parseInt(btn.getAttribute("data-id"), 10);
            await deleteFriend(id);
        });
    });

    renderFriendsPagination(total, totalPages);
}
function renderFriendsPagination(total, totalPages) {
    const container = $("friendsPagination");
    if (total <= PAGE_SIZE) {
        container.innerHTML = "";
        return;
    }

    container.innerHTML = "";

    const info = document.createElement("span");
    info.textContent = `Página ${friendsPage} de ${totalPages}`;
    container.appendChild(info);

    const prev = document.createElement("button");
    prev.textContent = "Anterior";
    prev.disabled = friendsPage === 1;
    prev.addEventListener("click", () => {
        if (friendsPage > 1) {
            friendsPage--;
            renderFriendsTable();
        }
    });
    container.appendChild(prev);

    const next = document.createElement("button");
    next.textContent = "Próxima";
    next.disabled = friendsPage === totalPages;
    next.addEventListener("click", () => {
        if (friendsPage < totalPages) {
            friendsPage++;
            renderFriendsTable();
        }
    });
    container.appendChild(next);
}


function startEditFriend(id) {
    const friend = cachedFriends.find((f) => f.id === id);
    if (!friend) return;

    editingFriendId = id;
    $("friendIdEditing").value = id;

    $("friendName").value = friend.name ?? "";
    $("friendEmail").value = friend.email ?? "";
    $("friendPhone").value = friend.phone ?? "";

    $("friendSubmitBtn").textContent = "Atualizar amigo";

    // foco no formulário
    $("friendName").scrollIntoView({ behavior: "smooth", block: "center" });
}
async function deleteFriend(id) {
    if (!confirm("Tem certeza que deseja excluir este amigo?")) return;

    try {
        await apiRequest(`/Friends/${id}`, {
            method: "DELETE"
        });

        // se estava editando esse amigo, resetar form
        if (editingFriendId === id) {
            editingFriendId = null;
            $("friendIdEditing").value = "";
            $("createFriendForm").reset();
            $("friendSubmitBtn").textContent = "Salvar amigo";
        }

        await loadFriends();
        await loadLoanSelects(); // atualiza selects de empréstimo
    } catch (err) {
        $("friendsError").textContent = err.message;
    }
}


$("reloadFriendsBtn").addEventListener("click", loadFriends);

$("createFriendForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    $("friendCreateMessage").textContent = "";
    $("friendCreateError").textContent = "";

    try {
        const { nome, email, phone } = validarFriendForm();

        const body = { name: nome, email, phone };

        if (editingFriendId == null) {
            // CRIAR
            await apiRequest("/Friends", {
                method: "POST",
                body: JSON.stringify(body)
            });
            $("friendCreateMessage").textContent = "Amigo cadastrado com sucesso.";
        } else {
            // EDITAR
            await apiRequest(`/Friends/${editingFriendId}`, {
                method: "PUT",
                body: JSON.stringify(body)
            });
            $("friendCreateMessage").textContent = "Amigo atualizado com sucesso.";
        }

        // resetar estado
        editingFriendId = null;
        $("friendIdEditing").value = "";
        $("friendSubmitBtn").textContent = "Salvar amigo";
        e.target.reset();

        await loadFriends();
        await loadLoanSelects();
    } catch (err) {
        $("friendCreateError").textContent = err.message;
    }
});


// -------- JOGOS --------

async function loadGames() {
    $("gamesError").textContent = "";
    const tbody = $("gamesTable").querySelector("tbody");
    tbody.innerHTML = "<tr><td colspan='4'>Carregando...</td></tr>";

    try {
        const games = await apiRequest("/Games");
        cachedGames = games || [];
        gamesPage = 1;
        updateStats();
        renderGamesTable();
    } catch (err) {
        $("gamesError").textContent = err.message;
        tbody.innerHTML = "";
        $("gamesPagination").innerHTML = "";
    }
}
function renderGamesTable() {
    const tbody = $("gamesTable").querySelector("tbody");

    if (!cachedGames || cachedGames.length === 0) {
        tbody.innerHTML = "<tr><td colspan='4'>Nenhum jogo encontrado.</td></tr>";
        $("gamesPagination").innerHTML = "";
        return;
    }

    const total = cachedGames.length;
    const totalPages = Math.ceil(total / PAGE_SIZE);
    if (gamesPage > totalPages) gamesPage = totalPages;

    const start = (gamesPage - 1) * PAGE_SIZE;
    const pageItems = cachedGames.slice(start, start + PAGE_SIZE);

    tbody.innerHTML = "";
    pageItems.forEach((g) => {
        const name = g.name ?? g.title ?? "";
        const genre = Array.isArray(g.genre) ? g.genre[0] : g.genre ?? "";
        const platform = g.platform ?? "";
        const isLoaned = g.isLoaned ? "Sim" : "Não";

        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${name}</td>
            <td>${platform}</td>
            <td>${genre}</td>
            <td>${isLoaned}</td>
        `;
        tbody.appendChild(tr);
    });

    renderGamesPagination(total, totalPages);
}
function renderGamesPagination(total, totalPages) {
    const container = $("gamesPagination");
    if (total <= PAGE_SIZE) {
        container.innerHTML = "";
        return;
    }

    container.innerHTML = "";

    const info = document.createElement("span");
    info.textContent = `Página ${gamesPage} de ${totalPages}`;
    container.appendChild(info);

    const prev = document.createElement("button");
    prev.textContent = "Anterior";
    prev.disabled = gamesPage === 1;
    prev.addEventListener("click", () => {
        if (gamesPage > 1) {
            gamesPage--;
            renderGamesTable();
        }
    });
    container.appendChild(prev);

    const next = document.createElement("button");
    next.textContent = "Próxima";
    next.disabled = gamesPage === totalPages;
    next.addEventListener("click", () => {
        if (gamesPage < totalPages) {
            gamesPage++;
            renderGamesTable();
        }
    });
    container.appendChild(next);
}


$("reloadGamesBtn").addEventListener("click", loadGames);

$("createGameForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    $("gameCreateMessage").textContent = "";
    $("gameCreateError").textContent = "";

    const name = $("gameName").value.trim();
    const platform = $("gamePlatform").value.trim();
    const genresRaw = $("gameGenres").value.trim();
    const publishersRaw = $("gamePublishers").value.trim();

    if (!name || !genresRaw || !publishersRaw) {
        $("gameCreateError").textContent = "Preencha nome, gênero(s) e publicadora(s).";
        return;
    }

    const genres = genresRaw.split(",").map((s) => s.trim()).filter(Boolean);
    const publishers = publishersRaw.split(",").map((s) => s.trim()).filter(Boolean);

    if (genres.length === 0 || publishers.length === 0) {
        $("gameCreateError").textContent = "Informe pelo menos um gênero e uma publicadora.";
        return;
    }

    try {
        await apiRequest("/Games", {
            method: "POST",
            body: JSON.stringify({
                name,
                platform: platform || null,
                genre: genres,        // ou Genres, depende do DTO da API
                publishers: publishers,
                externalSourceId: null
            })
        });

        $("gameCreateMessage").textContent = "Jogo cadastrado com sucesso.";
        e.target.reset();
        await loadGames();
        await loadLoanSelects();
    } catch (err) {
        $("gameCreateError").textContent = err.message;
    }
});

// -------- EMPRÉSTIMOS --------

async function loadLoanSelects() {
    const friendSelect = $("loanFriendSelect");
    const gameSelect = $("loanGameSelect");

    // Friends
    friendSelect.innerHTML = '<option value="">Selecione um amigo</option>';
    (cachedFriends || []).forEach(f => {
        const opt = document.createElement("option");
        opt.value = String(f.id);          // 👈 TEM QUE SER o id do friend
        opt.textContent = f.name;
        friendSelect.appendChild(opt);
    });

    // Games
    gameSelect.innerHTML = '<option value="">Selecione um jogo</option>';
    (cachedGames || []).forEach(g => {
        const opt = document.createElement("option");
        opt.value = String(g.id);          // 👈 AQUI É O PONTO: usa g.id
        opt.textContent = g.name;          // pode colocar gênero junto se quiser
        gameSelect.appendChild(opt);
    });
}


async function loadLoans() {
    resetLoanEditState();
    $("loansError").textContent = "";
    const tbody = $("loansTable").querySelector("tbody");
    tbody.innerHTML = "<tr><td colspan='5'>Carregando...</td></tr>";

    try {
        const loans = await apiRequest("/Loans?onlyActive=true");
        cachedActiveLoans = loans || [];
        loansPage = 1;
        updateStats();
        renderLoansTable();
    } catch (err) {
        $("loansError").textContent = err.message;
        tbody.innerHTML = "";
        $("loansPagination").innerHTML = "";
    }
}
function renderLoansTable() {
    const tbody = $("loansTable").querySelector("tbody");

    if (!cachedActiveLoans || cachedActiveLoans.length === 0) {
        tbody.innerHTML = "<tr><td colspan='5'>Nenhum empréstimo ativo.</td></tr>";
        $("loansPagination").innerHTML = "";
        return;
    }

    const total = cachedActiveLoans.length;
    const totalPages = Math.ceil(total / PAGE_SIZE);
    if (loansPage > totalPages) loansPage = totalPages;

    const start = (loansPage - 1) * PAGE_SIZE;
    const pageItems = cachedActiveLoans.slice(start, start + PAGE_SIZE);

    tbody.innerHTML = "";
    tbody.innerHTML = "";
    pageItems.forEach((l) => {
        const friendName = l.friendName ?? l.friendId;
        const gameTitle = l.gameTitle ?? l.gameId;
        const loanDate = l.loanDate ? new Date(l.loanDate).toLocaleDateString("pt-BR") : "";
        const expected = l.expectedReturnDate
            ? new Date(l.expectedReturnDate).toLocaleDateString("pt-BR")
            : "";

        const tr = document.createElement("tr");
        tr.dataset.loanId = l.id ?? l.loanId;              // tolera id ou loanId
        tr.dataset.friendId = l.friendId ?? "";            // se vier
        tr.dataset.friendName = l.friendName ?? "";
        tr.dataset.gameId = l.gameId ?? "";
        tr.dataset.gameTitle = l.gameTitle ?? "";
        tr.dataset.expectedReturn = l.expectedReturnDate ?? "";

        tr.innerHTML = `
            <td>${friendName}</td>
            <td>${gameTitle}</td>
            <td>${loanDate}</td>
            <td>${expected}</td>
            <td>
                <button class="small-btn edit-loan">Editar</button>
                <button class="small-btn danger return-btn" data-loan-id="${tr.dataset.loanId}">Devolver</button>
            </td>
        `;
        tbody.appendChild(tr);
    });


    tbody.querySelectorAll(".return-btn").forEach((btn) => {
        btn.addEventListener("click", async () => {
            const id = btn.getAttribute("data-loan-id");
            try {
                const res = await apiRequest(`/Loans/${id}/return`, {
                    method: "POST"
                });
                $("loanCreateMessage").textContent = res.message || "Empréstimo devolvido.";
                await loadLoans();
                await loadGames();
                await loadLoanSelects();
            } catch (err) {
                $("loanError").textContent = err.message;
            }
        });
    });
    tbody.querySelectorAll(".edit-loan").forEach((btn) => {
    btn.addEventListener("click", () => {
        const tr = btn.closest("tr");
        if (!tr) return;

        startEditLoan({
            id: tr.dataset.loanId,
            friendId: tr.dataset.friendId,
            friendName: tr.dataset.friendName,
            gameId: tr.dataset.gameId,
            gameTitle: tr.dataset.gameTitle,
            expectedReturn: tr.dataset.expectedReturn
        });
    });
});
tbody.querySelectorAll(".return-btn").forEach((btn) => {
    btn.addEventListener("click", async () => {
        const id = btn.getAttribute("data-loan-id");
        try {
            const res = await apiRequest(`/Loans/${id}/return`, {
                method: "POST"
            });
            $("loanCreateMessage").textContent = res.message || "Empréstimo devolvido.";
            await loadLoans();
            await loadGames();
            await loadLoanSelects();
        } catch (err) {
            $("loanError").textContent = err.message;
        }
    });
});

    renderLoansPagination(total, totalPages);
}

function startEditLoan(data) {
    // data vem direto da <tr>
    console.log("Editando empréstimo:", data);

    editingLoanId = data.id;

    // amigo
    const friendSelect = $("loanFriendSelect");
    if (data.friendId) {
        friendSelect.value = data.friendId;
    } else if (data.friendName) {
        const opt = Array.from(friendSelect.options)
            .find(o => o.text.trim() === data.friendName.trim());
        if (opt) friendSelect.value = opt.value;
    }

    // jogo
    const gameSelect = $("loanGameSelect");
    if (data.gameId) {
        gameSelect.value = data.gameId;
    } else if (data.gameTitle) {
        const opt = Array.from(gameSelect.options)
            .find(o => o.text.trim() === data.gameTitle.trim());
        if (opt) gameSelect.value = opt.value;
    }

    // data prevista – form `input type="date"` espera yyyy-MM-dd
    if (data.expectedReturn) {
        const d = new Date(data.expectedReturn);
        if (!isNaN(d.getTime())) {
            $("loanExpectedReturn").value = d.toISOString().slice(0, 10);
        }
    } else {
        $("loanExpectedReturn").value = "";
    }

    // só edição de data
    friendSelect.disabled = true;
    gameSelect.disabled = true;

    $("loanSubmitBtn").textContent = "Atualizar empréstimo";
    $("loanEditHint").style.display = "block";
    $("loanCancelEditBtn").style.display = "inline-block";
    $("loanCancelEditBtn").addEventListener("click", () => {
            resetLoanEditState();
        });
    $("loanExpectedReturn").scrollIntoView({ behavior: "smooth", block: "center" });
}




function renderLoansPagination(total, totalPages) {
    const container = $("loansPagination");
    if (total <= PAGE_SIZE) {
        container.innerHTML = "";
        return;
    }

    container.innerHTML = "";

    const info = document.createElement("span");
    info.textContent = `Página ${loansPage} de ${totalPages}`;
    container.appendChild(info);

    const prev = document.createElement("button");
    prev.textContent = "Anterior";
    prev.disabled = loansPage === 1;
    prev.addEventListener("click", () => {
        if (loansPage > 1) {
            loansPage--;
            renderLoansTable();
        }
    });
    container.appendChild(prev);

    const next = document.createElement("button");
    next.textContent = "Próxima";
    next.disabled = loansPage === totalPages;
    next.addEventListener("click", () => {
        if (loansPage < totalPages) {
            loansPage++;
            renderLoansTable();
        }
    });
    container.appendChild(next);
}

function resetLoanEditState() {
    editingLoanId = null;
    $("loanExpectedReturn").value = "";
    $("loanFriendSelect").disabled = false;
    $("loanGameSelect").disabled = false;
    $("loanSubmitBtn").textContent = "Emprestar jogo";
    $("loanEditHint").style.display = "none";
    $("loanCancelEditBtn").style.display = "none";
}


$("reloadLoansBtn").addEventListener("click", loadLoans);

$("createLoanForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    $("loanCreateMessage").textContent = "";
    $("loanError").textContent = "";

    const expectedStr = $("loanExpectedReturn").value;

    if (!expectedStr) {
        $("loanError").textContent = "Informe a data prevista de devolução.";
        return;
    }

    const friendId = parseInt($("loanFriendSelect").value, 10);
    const gameId = parseInt($("loanGameSelect").value, 10);
    const expectedDateIso = new Date(expectedStr).toISOString();

    try {
        if (editingLoanId == null) {
            // CRIAR novo empréstimo (POST)
            const res = await apiRequest("/Loans", {
                method: "POST",
                body: JSON.stringify({
                    friendId,
                    gameId,
                    expectedReturnDate: expectedDateIso
                })
            });

            $("loanCreateMessage").textContent = res.message || "Empréstimo criado.";
        } else {
            // EDITAR empréstimo existente (PUT)
            const res = await apiRequest(`/Loans/${editingLoanId}`, {
                method: "PUT",
                body: JSON.stringify({
                    expectedReturnDate: expectedDateIso
                })
            });

            $("loanCreateMessage").textContent = res.message || "Empréstimo atualizado.";
        }

        await loadLoans();
        await loadGames();
        await loadLoanSelects();
        resetLoanEditState();
    } catch (err) {
        $("loanError").textContent = err.message;
    }
});


// -------- Inicialização: tentar recuperar sessão --------

(function init() {

    const token = localStorage.getItem("gamesloan_token");
    const user = localStorage.getItem("gamesloan_user");

    // Tabs de autenticação
    document.querySelectorAll(".auth-tab").forEach(tab => {
        tab.addEventListener("click", () => {

            // troca as abas
            document.querySelectorAll(".auth-tab").forEach(t =>
                t.classList.remove("active")
            );
            tab.classList.add("active");

            // troca os painéis
            const targetId = tab.dataset.target; // loginForm ou registerForm

            document.querySelectorAll(".auth-panel").forEach(panel =>
                panel.classList.remove("active")
            );

            document.getElementById(targetId).classList.add("active");
        });
    });

    // estado inicial: logado ou deslogado
    if (token && user) {
        setLoggedInState(user, token);
    } else {
        setLoggedOutState();
    }

})();

