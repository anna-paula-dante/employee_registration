# People Manager — Execução rápida (Docker)

Aplicação completa (API .NET 8 + Postgres + Frontend React/Vite) com Docker.

## Requisitos
- Docker Desktop (Windows/Mac) ou Docker + Compose
- Portas livres: API 9999, Front 5173 (dev) ou 8081 (prod)

## Subir tudo com Docker

1) Backend (API + Postgres)
```bash
cd backend
# primeira vez ou reset:
docker compose down -v
# subir banco e API (porta 8001)
docker compose up -d --build
# opcional: ver logs da API (seed do admin)
docker logs -f people_api_fix
```
- Swagger: http://localhost:8001/swagger
- Health: http://localhost:8001/health
- Admin padrão:
  - Email: admin@peoplemanager.com
  - Senha: Admin@12345

2a) Frontend Dev (hot-reload)
```bash
cd fronted/peoplemanager-frontend
# define a URL da API (se quiser mudar, padrão já aponta para 9999)
set VITE_API_BASE_URL=http://localhost:8001   # Windows PowerShell
# export VITE_API_BASE_URL=http://localhost:8001   # macOS/Linux

docker compose up -d fe-dev
```
- Abrir: http://localhost:5173

2b) Frontend Prod (build + Nginx)
```bash
cd fronted/peoplemanager-frontend
# opcional: definir API no build
docker compose build fe-prod --build-arg VITE_API_BASE_URL=http://localhost:8001
# subir
docker compose up -d fe-prod
```
- Abrir: http://localhost:8081

## Fluxo de login
1. Acesse o frontend (Dev 5173 ou Prod 8081)
2. Entre com o admin:
   - Email: admin@peoplemanager.com
   - Senha: Admin@12345
3. Gerencie funcionários (CRUD)

## Variáveis de ambiente úteis
- Backend (arquivo backend/docker-compose.yml):
  - Jwt__Key: segredo do JWT (>= 32 chars)
  - ADMIN_*: dados do admin seed (senha, email etc.)
- Frontend (lidas pelo Vite):
  - VITE_API_BASE_URL: URL da API (default http://localhost:8001)
  - VITE_API_ROLE_IS_NUMERIC: true/false (se o backend envia role como número)

## Problemas comuns
- CORS no login: já configurado no backend para http://localhost:5173. Se usar outra origem, defina `FRONTEND_URLS` na API.
- Seed não atualiza senha: se o admin já existir, o seed não sobrescreve. Para resetar, rode `docker compose down -v` no backend e suba novamente.
- JWT inválido por chave curta: aumente `Jwt__Key` para 32+ caracteres.

## Documentação detalhada
- Backend: veja `backend/README.md`
- Frontend: veja `fronted/peoplemanager-frontend/README.md`
