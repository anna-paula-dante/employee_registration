# PeopleManager

API REST (.NET 8) para gestão de funcionários com autenticação JWT, RBAC, CRUD completo, testes, documentação Swagger e Docker (PostgreSQL).

## Requisitos atendidos
- Funcionário com: nome, sobrenome, e-mail (único), documento (único), **>= 2 telefones**, data de nascimento (valida **>= 18 anos**), senha (hash Bcrypt), papel (Employee/Leader/Director) e gestor (gestor é funcionário).
- **RBAC**: você **não pode criar** alguém **com papel superior** ao seu. Líder não cria Diretor; Funcionário não cria Líder/ Diretor.
- **API REST .NET 8**, **CRUD completo** (`/api/v1/employees`).
- **Banco no Docker** (PostgreSQL) e compose pronto.
- **Autenticação JWT**, **Swagger** com Bearer, **Serilog** para logs.
- **Testes** (xUnit + WebApplicationFactory + InMemory EF) cobrindo login, RBAC e validação de idade.

## Rodando com Docker
```bash
docker compose down -v
docker compose up --build -d
docker logs -f people_api_fix
```
No primeiro start, verá nos logs:  
`[seed] admin created -> admin@peoplemanager.com`  

### Healthcheck
```
curl http://localhost:8001/health
```

### Swagger
Abrir: `http://localhost:8001/swagger`

## Login padrão
- Email: `admin@peoplemanager.com`
- Senha: `Admin@12345`

Se quiser alterar, edite as variáveis de ambiente no `docker-compose.yml` (`ADMIN_*`).

## Endpoints principais

### Autenticação
`POST /api/v1/auth/login`  
Body:
```json
{ "emailOrDocument": "admin@peoplemanager.com", "password": "Admin@12345" }
```
Retorna `access_token` (Bearer).

### Funcionários
- `GET /api/v1/employees?page=1&pageSize=20&search=...`
- `GET /api/v1/employees/{id}`
- `POST /api/v1/employees` (Bearer) – cria funcionário. **Phones >= 2**. **RBAC**.
- `PUT /api/v1/employees/{id}` (Bearer) – atualiza. **RBAC**.
- `DELETE /api/v1/employees/{id}` (Bearer) – remove. **RBAC**.

### Exemplo de criação
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@x.com",
  "documentNumber": "12345678900",
  "birthDate": "1990-01-01",
  "password": "John@123",
  "role": 0,
  "managerId": null,
  "phones": ["+55 11 90000-0001", "+55 11 90000-0002"]
}
```

## Rodando os testes (local)
> Os testes usam banco **InMemory** e não dependem do Docker.
```bash
dotnet test PeopleManager.Api.Tests
```

## Estrutura
- **PeopleManager.Domain**: entidades e enum.
- **PeopleManager.Infrastructure**: `AppDbContext` (EF Core, Postgres).
- **PeopleManager.Application**: DTOs (requests).
- **PeopleManager.Api**: Program, JWT, Controllers (Auth, Employees), Swagger, Dockerfile.
- **PeopleManager.Api.Tests**: xUnit, WebApplicationFactory, testes de Auth, RBAC e idade.

## Observações
- O seed do admin lê `ADMIN_*` (e também `Admin__*`); se não definir, usa os padrões do compose.
- O projeto inicializa o banco chamando `MigrateAsync()` e, caso não haja migração, usa `EnsureCreatedAsync()` para garantir o schema.
