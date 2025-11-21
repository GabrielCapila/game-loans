# GamesLoan API

API para gerenciar empréstimos de jogos entre amigos.

## Visão Geral da Arquitetura

Camadas (Clean Architecture simplificada / CQRS):
- Domain: Entidades ricas (`Friend`, `Game`, `Loan`, `User`) + Exceptions e Interfaces de Repositório.
- Application: Casos de uso (Commands e Queries via MediatR), DTOs e regras adicionais.
- Infrastructure: Persistência (EF Core MySQL), Repositórios concretos, Configurações de mapeamento, Autenticação (JWT), Integrações externas (Playstation API), Hosted Service de importação.
- Api: Controllers REST, Middleware de exceções, Configuração de DI, Swagger, Autenticação.
- Tests: Testes unitários dos handlers principais.

Padrões usados:
- CQRS + MediatR (separação de leitura/escrita).
- Repository Pattern.

- JWT Bearer Authentication.
- Hosted Service para importação inicial de jogos.
- Middleware global de tratamento de exceções.

## Principais Entidades
- Friend: Nome, Email, Phone, IsActive. Regras de validação de email/phone e soft-delete.
- Game: Nome, Publishers, Genre, ExternalSourceId, IsLoaned. Métodos para emprestar/devolver.
- Loan: GameId, FriendId, LoanDate, ExpectedReturnDate, ActualReturnDate, Status (Open/Returned/Cancelled). Regras para retorno e atualização de data.
- User: Username, PasswordHash. Hash de senha via `IPasswordHasher`.

## Fluxos de Negócio
1. Registro de usuário (gera hash da senha e salva). 
2. Login (valida senha, gera JWT). 
3. Criação de amigo (validação de email/telefone e unicidade de email). 
4. Criação de jogo (manual ou importado via HostedService de fonte externa). 
5. Empréstimo de jogo (verifica se jogo existe e não está emprestado; marca como loaned). 
6. Devolução (registra data de retorno, marca jogo como disponível). 
7. Atualização da data prevista de retorno (não pode alterar se já foi devolvido). 
8. Listagens (todos os empréstimos, somente ativos, por amigo). 

## Tratamento de Erros
Middleware `ExceptionHandlingMiddleware` converte exceções conhecidas em respostas JSON:
- `NotFoundException` -> 404
- `DomainException` -> 409
- `UnauthorizedException` -> 401
- Demais -> 500
Formato:
```json
{
  "statusCode": 404,
  "error": "Mensagem detalhada"
}
```

## Autenticação
- JWT Bearer configurado em `Program.cs` lendo seção `Jwt` (`Issuer`, `Audience`, `SecretKey`).
- Endpoints de Auth liberados com `[AllowAnonymous]`.
- Demais controllers anotados com `[Authorize]`.

## Endpoints
Base URL: `/api`.

### Auth
POST `/api/auth/register`
Body: `{ "username": "user", "password": "senha" }`
Retorno: `201` `{ id, username }`

POST `/api/auth/login`
Body: `{ "username": "user", "password": "senha" }`
Retorno: `200` `{ token, username }`
Header para chamadas autenticadas: `Authorization: Bearer <token>`

### Friends (autenticado)
POST `/api/friends`
Body: `{ "name": "João", "email": "joao@mail.com", "phone": "+551199999999" }`
Retorno: `201` (Location com id).

GET `/api/friends`
Retorno: `200` `[ { id, name, email, phone, isActive, createdAt } ]`

GET `/api/friends/{id}`
Retorno: `200` ou `404`.

PUT `/api/friends/{id}`
Body: `{ "name": "Novo", "email": "novo@mail.com", "phone": "+55..." }`
Retorno: `204` ou `404`.

DELETE `/api/friends/{id}` (soft-delete)
Retorno: `204` ou `404`.

### Games (autenticado)
GET `/api/games`
Retorno: `200` `[ { id, name, publishers, genre, externalSourceId, isLoaned, createdAt } ]`

POST `/api/games`
Body: `{ "name": "Game X", "publishers": ["Ubisoft"], "genre": ["Action"], "externalSourceId": "123" }`
Retorno: `201`.

### Loans (autenticado)
POST `/api/loans`
Body: `{ "friendId": 1, "gameId": 10, "expectedReturnDate": "2025-12-31" }`
Retorno: `201` `{ loanId, friendId, gameId, message }`

POST `/api/loans/{loanId}/return`
Retorno: `200` `{ loanId, friendId, friendName, gameId, gameTitle, message }`

GET `/api/loans?onlyActive=true|false`
Retorno: `200` `[ { id, friendId, friendName, gameId, gameTitle, loanDate, expectedReturnDate, returnDate, status } ]`

GET `/api/loans/by-friend/{friendId}`
Retorno: `200` (lista por amigo).

PUT `/api/loans/{id}`
Body: `{ "expectedReturnDate": "2025-01-15" }`
Retorno: `200` `{ id, expectedReturnDate, message }`

## Hosted Service de Importação
`GamesImportHostedService` executa na inicialização:
- Chama API externa (SampleApis/Playstation).
- Evita duplicação consultando `ExternalSourceId` já existentes.
- Persiste apenas novos jogos.

## Persistência
- EF Core + MySQL.
- Mapeamentos via `IEntityTypeConfiguration` (Game, Friend, Loan, User).
- Índices: `ExternalSourceId` em games, `Username` (único) em users, `GameId/FriendId` em loans.

## DTOs vs Entidades
- Controllers recebem DTOs de entrada (`CreateFriendRequest`, `CreateLoanRequest`, etc.).
- Handlers retornam DTOs de saída (`LoanDetailsDto`, `LoanCreatedDto`, etc.).


## Configuração JWT
No `appsettings.json`:
```json
"Jwt": {
  "Issuer": "GamesLoanIssuer",
  "Audience": "GamesLoanAudience",
  "SecretKey": "super-secret-key-change-this"
}
```
Alterar `SecretKey` para produção e usar variável de ambiente.

## CORS
Configurado como permissivo (`AllowAnyOrigin`). Ajustar para produção.

## Logs
- EF Core filtrado para Warning.
- Import job loga início/fim e quantidade importada.
- Middleware loga warnings e erros inesperados.

## Testes
Projeto `Tests` cobre:
- Registro e Login.
- Criação de Friend e Game.
- Fluxo de empréstimo e devolução.
- Regras de entidades (`GameTests`).

## Possíveis Evoluções
- Filtros em listagens.
- Refresh token / expiração configurável.
- Auditoria e histórico de empréstimos por jogo.
- Roles e permissões.
- Cache para listas de jogos.
- Documentar contratos externos da Playstation API.

## Estrutura de Pastas (resumida)
```
src/
  Domain/
  Application/
  Infrastructure/
  Api/
Tests/
README.md
```

## Formato de Erros (Exemplos)
404:
```json
{ "statusCode": 404, "error": "Friend with id 99 was not found." }
```
409:
```json
{ "statusCode": 409, "error": "The game 'XYZ' is already loaned." }
```
401:
```json
{ "statusCode": 401, "error": "Invalid username or password." }
```
500:
```json
{ "statusCode": 500, "error": "An unexpected error occurred." }
```

## Segurança / Produção
- Rotacionar `SecretKey`. 
- Restringir CORS. 
- Adicionar rate limiting e logs estruturados.

---
Projeto pronto para evolução incremental e extensão de novos casos de uso (novos Commands/Queries).