# Orquestração Docker - GamesLoan

Este README descreve como subir todo o ecossistema (Banco, API .NET e Front‑end) usando `docker-compose`.

## Sumário
- Visão Geral
- Estrutura de Pastas
- Serviços do compose
- Variáveis de ambiente
- Healthcheck do banco
- Build e subida dos containers
- Fluxo de inicialização (migrações + hosted service)
- Acesso aos serviços
- Comandos úteis
- Personalizações
- Troubleshooting

## Visão Geral
O `docker-compose.yml` orquestra três serviços:
1. `db` (MariaDB 11) – persiste dados da aplicação.
2. `api` (GamesLoan API .NET 8) – expõe endpoints REST e aplica migrações automaticamente no startup.
3. `front` (Aplicação front-end) – consome a API.

A API depende do banco estar saudável (healthcheck) antes de iniciar; o front depende da API.

## Estrutura de Pastas (raiz do repo)
```
Tanto a api quanto o front possuem seus Dockerfile
./docker-compose.yml
./games-loan-api/        -> Código .NET (Domain, Application, Infrastructure, Api)
./front-end/             -> Código do front 
```

## Serviços do compose
### db
- Imagem: `mariadb:11`
- Porta exposta: `3306`
- Healthcheck garante que o InnoDB esteja inicializado antes da API subir.

### api
- Contexto de build: `./games-loan-api`
- Usa `Dockerfile` dentro dessa pasta.
- Porta interna 8080 publicada em `7108` (host).
- Aplica `db.Database.Migrate()` no startup, criando/atualizando schema.
- Executa `GamesImportHostedService` para importar jogos externos (evitando duplicação).

### front
- Contexto de build: `./front-end`
- Usa `Dockerfile` dentro dessa pasta.
- Porta exposta: `8080` (host) -> `80` (container). A aplicação deve apontar para `http://localhost:7108` para acessar a API. (Já configurado)

## Variáveis de Ambiente
Definidas em `docker-compose.yml` (serviço `api`):
- `ConnectionStrings__DefaultConnection`: string de conexão para MariaDB dentro da rede dos containers (`server=db`).
- `Jwt__Issuer`, `Jwt__Audience`, `Jwt__SecretKey`: parâmetros para geração/validação dos tokens JWT.
- `ASPNETCORE_ENVIRONMENT`: definido como `Production` (alterar para `Development` se quiser logs mais verbosos e Swagger sem HTTPS).

Banco (`db`):
- `MYSQL_ROOT_PASSWORD`: senha do usuário root.
- `MYSQL_DATABASE`: database inicial criada.
- `MYSQL_USER` / `MYSQL_PASSWORD`: usuário de aplicação usado pela API.

## Healthcheck do Banco
```
healthcheck:
  test: ["CMD", "healthcheck.sh", "--connect", "--innodb_initialized"]
  interval: 3s
  timeout: 5s
  retries: 10
  start_period: 20s
```
O serviço `api` só inicia quando o status do `db` é `healthy`, evitando falhas de conexão inicial.

## Build e Subida
Comando único:
```
docker compose up --build
```
(Use `docker-compose` se sua versão não suportar o plugin compose.)

Para rodar em background:
```
docker compose up --build -d
```

Para parar tudo:
```
docker compose down
```

## Fluxo de Inicialização da API
1. Conexão com MariaDB (após healthcheck).
2. Executa `db.Database.Migrate()` aplicando migrações pendentes.
3. `GamesImportHostedService` chama API externa (SampleApis/Playstation) e importa novos jogos.
4. Swagger disponível (em produção somente se configurado – já exposto). Endereço: `http://localhost:7108/swagger`.

## Acesso aos Serviços
- API: `http://localhost:7108`
- Swagger: `http://localhost:7108/swagger`
- Front-end: `http://localhost:8080`

Autenticação JWT:
1. Registrar usuário: POST `/api/auth/register`
2. Login: POST `/api/auth/login` -> recebe `token`
3. Usar header: `Authorization: Bearer <token>` nas demais rotas.

## Comandos Úteis
Logs em tempo real:
```
docker compose logs -f api
```
Rebuild apenas da API:
```
docker compose build api && docker compose up -d api
```
Executar um comando dentro do container da API (ex: listar migrações):
```
docker compose exec api ls /app
```
Adicionar uma migração nova (criando no container – prefere localmente):
```
docker compose exec api dotnet ef migrations add NomeDaMigracao --project src/Infrastructure --startup-project src/Api
```
Aplicar migrações manualmente:
```
docker compose exec api dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

## Personalizações
- Alterar porta da API: modifique `ports: - "7108:8080"` (lado esquerdo é host).
- Alterar segredos JWT: editar valores em `docker-compose.yml` ou usar arquivos `.env`.
- Limitar CORS: ajustar política em `Program.cs` (trocar `AllowAnyOrigin()` por origens específicas).
- Produção: usar imagens reduzidas (multi-stage build já recomendado). Garanta que a `SecretKey` seja forte e rotacionada.

## Atualizando Dependências
Após alterar código ou pacotes:
```
docker compose build api
```
O front precisa rebuild se houver mudança:
```
docker compose build front
```

## Rede Interna
Todos os serviços compartilham a rede default do compose.
- A API acessa o banco via host `db` (resolução de DNS interna).

## Troubleshooting
| Problema | Causa provável | Ação |
|----------|----------------|------|
| API falha ao subir / Connection refused | Banco ainda não saudável | Aguardar (healthcheck garante ordem) ou aumentar `start_period` |
| Erro de auth JWT | `SecretKey` diferente entre instâncias | Garantir chave consistente em todas as réplicas |
| Importação de jogos lenta | Latência API externa | Aumentar timeout do HttpClient ou executar job assíncrono posterior |
| Erro de charset no MariaDB | Config não definido | Adicionar env `MYSQL_CHARACTER_SET_SERVER` e `MYSQL_COLLATION_SERVER` se necessário |
| Migração não aplica | Falta de permissões ou DB não existe | Confirmar database criado e usuário tem acesso |

## Limpeza Total
Remove containers + volumes anônimos (se você criou volumes nomeados, incluir flag):
```
docker compose down -v
```

## Segurança
- Nunca commitar segredos reais (rotacionar `Jwt__SecretKey`).
- Usar TLS (reverse proxy como Traefik / Nginx) em produção.
- Adicionar rate limiting (ex: `AspNetCoreRateLimit`).

---
Ambiente Docker pronto para desenvolvimento rápido e testes integrados.
