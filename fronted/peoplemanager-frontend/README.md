
# People Manager — Front-end (React + Vite + TS)

UI profissional em tons neutros para consumir a sua API (.NET 8). Inclui autenticação JWT, RBAC na interface, CRUD de funcionários com validações, testes, Tailwind e Docker.

## Requisitos
- Node 20+
- API rodando em `http://localhost:8080` (ajuste `.env` se necessário)

## Instalação e execução (dev)
```bash
cp .env.example .env        # edite se quiser
npm i
npm run dev                 # http://localhost:5173
```

## Variáveis de ambiente
- `VITE_API_URL` — URL base da API (padrão: `http://localhost:8080`)
- `VITE_API_ROLE_IS_NUMERIC` — `true` se a API retorna role como número (0/1/2), `false` se string (`Employee|Leader|Director`).

## Build produção (Docker)
```bash
docker compose up -d fe-prod   # http://localhost:8081
```
> No Mac/Windows, o Nginx acessa a API externa via `http://host.docker.internal:8080` se você setar `VITE_API_URL` no build.

## Estrutura
- `src/lib/api.ts` — axios + interceptor Bearer
- `src/context/AuthContext.tsx` — estado de autenticação e RBAC
- `src/components/ProtectedRoute.tsx` — protege rotas por login/role
- `src/pages/Employees` — Listagem e Formulário (CRUD)
- `src/validations/employeeSchemas.ts` — regras: maioridade, min 2 telefones, senha forte

## Testes
```bash
npm run test
```

## Fluxo
1. **Login** (`/login`) → JWT salvo no localStorage.
2. **Listar** funcionários → buscar, editar, excluir.
3. **Criar/Editar** valida: maioridade (≥ 18), >= 2 telefones, e impede promover acima do papel atual do usuário (RBAC).

## Estilo
- Tailwind com tons **neutros** e foco em legibilidade.
- Layout responsivo, cartões translúcidos, sombras suaves.

## Integração com a API
- Login: `POST /api/v1/auth/login { emailOrDocument, password }`
- Employees:
  - `GET /api/v1/employees?page=&pageSize=&search=`
  - `POST /api/v1/employees`
  - `GET /api/v1/employees/{id}`
  - `PUT /api/v1/employees/{id}`
  - `DELETE /api/v1/employees/{id}`
Tenha certeza de que a API retorna/aceita: `{ phones: [{ number: string }], role: 0|1|2 }` e `birthDate` em ISO.

---

Se quiser, posso adicionar paginação avançada, tema claro/escuro e testes de integração (RTL) em outra etapa.
