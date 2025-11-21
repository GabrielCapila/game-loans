# Front-end GamesLoan

Foi utilizado IA para a ajudar/desenvolver uma interface web simples (HTML/CSS/JS puro) para consumir a GamesLoan API.

## Objetivos
- Autenticar usuário (registro + login) e armazenar token JWT.
- Gerenciar amigos (CRUD parcial: criar, listar, editar, excluir). *Soft-delete na API.*
- Gerenciar jogos (criar e listar). Importação adicional ocorre via HostedService da API.
- Criar, listar, editar (apenas data prevista) e devolver empréstimos.
- Exibir estatísticas rápidas (quantidade de amigos, jogos e empréstimos ativos).

## Stack
- HTML5 + CSS (estilização responsiva simples). 
- JavaScript ES6 sem frameworks.
- Nginx (imagem `nginx:alpine`) para servir arquivos estáticos em produção via Docker.

## Estrutura de Arquivos
```
front-end/
  index.html       -> Estrutura da página e containers de views
  style.css        -> Estilos (layout, tabs, tabelas, responsividade, animações)
  script.js        -> Lógica de autenticação, CRUD e integração com a API
  config.js        -> Config global (BASE URL da API)
  Dockerfile       -> Build do front (copia estáticos para imagem Nginx)
```

## Fluxo de Execução
1. Carrega `config.js` definindo `window.APP_CONFIG.API_BASE_URL` apontando para a API (`http://<host>:7108/api`).
2. Carrega `script.js` que:
   - Lê token e usuário salvos em `localStorage` para restaurar sessão.
   - Configura listeners para formulários e abas.
   - Define funções de requisição HTTP (`apiRequest`) que adicionam header `Authorization` quando autenticado.
   - Inicializa estado (logado / deslogado).

## Autenticação
- Registro: chama `POST /api/auth/register`.
- Login: chama `POST /api/auth/login` e salva `token` + `username` no `localStorage` (`gamesloan_token`, `gamesloan_user`).
- Logout: limpa `localStorage` e volta para a view de login.

## Organização da Página
- `loginView`: Exibida quando não autenticado. Possui abas para "Entrar" e "Criar conta".
- `mainView`: Exibida quando autenticado. Contém:
  - Barra superior com usuário logado e botão de sair.
  - Navegação por tabs: Amigos, Jogos, Empréstimos.
  - Cards e tabelas para cada seção.
  - Dashboard com três métricas: amigos, jogos e empréstimos ativos.

## Principais Funções (script.js)
### Utilidades
- `$` helper para `document.getElementById`.
- `apiRequest(path, options)`: Wrapper `fetch` com tratamento de erros (JSON) e inclusão de headers.

### Estado
- `authToken`, `currentUser`: mantêm sessão atual.
- `cachedFriends`, `cachedGames`, `cachedActiveLoans`: cache para evitar múltiplas chamadas e calcular stats.
- Controle de edição (`editingFriendId`, `editingLoanId`).
- Paginação (`friendsPage`, `gamesPage`, `loansPage`, `PAGE_SIZE`).

### Autenticação
- `setLoggedInState(username, token)`: Ajusta view e carrega dados iniciais.
- `setLoggedOutState()`: Reseta sessão e UI.

### Amigos
- `loadFriends()`: Chama `/Friends`, popula cache e renderiza tabela paginada.
- `renderFriendsTable() / renderFriendsPagination()`: Monta HTML dinamicamente.
- `startEditFriend(id)`: Carrega dados no formulário e altera botão para edição.
- `deleteFriend(id)`: Chama `DELETE /Friends/{id}`.
- Validação de form (`validarFriendForm`) com regex de email/telefone.

### Jogos
- `loadGames()`: Chama `/Games` e popula cache.
- `renderGamesTable() / renderGamesPagination()`.
- Criação de jogo (POST `/Games`) usando campos de texto separados por vírgula para gêneros e publishers.

### Empréstimos
- `loadLoans()`: Chama `/Loans?onlyActive=true`.
- `renderLoansTable()`: Lista empréstimos ativos, adiciona botões de editar e devolver.
- `startEditLoan(data)`: Carrega dados para editar apenas a data prevista (bloqueia selects friend/game).
- `resetLoanEditState()`: Volta ao estado de criação.
- Devolução: POST `/Loans/{id}/return`.
- Atualização: PUT `/Loans/{id}` (apenas data prevista).
- Criação: POST `/Loans`.
- `loadLoanSelects()`: Preenche selects com amigos e jogos 

### Stats
- `updateStats()`: Atualiza contadores no dashboard.

### Paginação
Cada tabela usa a mesma lógica: calcula `totalPages`, fatia o array de cache e exibe controles "Anterior" / "Próxima".

## Tratamento de Erros
- Erros de API exibidos em elementos `<div class="error">` ou `<p class="error-msg">` correspondentes.
- `apiRequest` tenta ler JSON; se não for 2xx, lança erro com `error` ou `message` retornados.

## Segurança (Limitações)
- Token JWT é armazenado em `localStorage` (vulnerável a XSS se não houver sanitização rigorosa). Para produção, preferir cookies `HttpOnly`.
- Sem proteção CSRF (não crítico em tokens Bearer, mas recomendável em cenários de cookies).
- Sem sanitização de entrada avançada; apenas validações básicas.

## Deploy com Docker
`Dockerfile` (Nginx):
```
FROM nginx:alpine
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY . .
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```
Build e run via docker-compose (serviço `front`) expõe porta `8080` apontando para Nginx servindo estes arquivos.

## Customização da URL da API
- Editar `front-end/config.js` para outra porta/dominio:
```js
window.APP_CONFIG = { API_BASE_URL: "https://meu-dominio/api" };
```
Ou dinamicamente usando variável de ambiente e mecanismo de substituição durante build (não implementado).

## Melhorias Futuras
- Componentização (Web Components ou framework: React/Vue/Svelte).
- Paginação e busca no servidor.
- Indicadores de loading por seção.
- Feedback visual para ações (toasts).
- Modo escuro/claro alternável.
- Internacionalização (labels em EN).
- Proteções adicionais (Content Security Policy, escaping de conteúdo dinâmico).

## Running Local Sem Docker
Abrir `index.html` em um servidor estático (ex: `npx serve`) e garantir que `API_BASE_URL` aponta para a API em execução local.

---
Front-end pronto para uso básico e fácil substituição por uma solução SPA mais robusta.
