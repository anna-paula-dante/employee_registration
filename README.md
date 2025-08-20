# People Manager

Aplicação completa para gestão de funcionários em uma empresa fictícia, composta por **API .NET 8 + PostgreSQL + React (Vite)**, containerizada com **Docker Compose**, com autenticação JWT, RBAC, CRUD de funcionários, validações e testes automatizados.

---

## Sumário
- [Visão Geral](#visão-geral)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Setup rápido com Docker](#setup-rápido-com-docker)
- [Ambientes](#ambientes)
- [Autenticação e RBAC](#autenticação-e-rbac)
- [Testes](#testes)
- [Documentação](#documentação)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Screenshots](#screenshots)
- [Contribuição](#contribuição)
- [Licença](#licença)

---

## Visão Geral

O **People Manager** é uma solução moderna para gestão de funcionários, permitindo que empresas cadastrem, editem e gerenciem colaboradores, com controle de permissões baseado em papéis (RBAC).  
O projeto foi desenvolvido aplicando **Clean Architecture, segurança com JWT, testes automatizados, containerização e boas práticas de mercado**.

---

## Tecnologias

### Backend
- .NET 8 Web API
- Entity Framework Core 8 + Npgsql
- FluentValidation
- JWT Bearer Authentication
- Serilog
- Swagger (Swashbuckle)

### Frontend
- React 18 + Vite
- TypeScript
- Axios + Interceptors JWT
- React Router DOM
- React Hook Form + Zod
- TailwindCSS
- Vitest

### Infraestrutura
- Docker & Docker Compose
- PostgreSQL 16
- Nginx

---

## Arquitetura

```
📦 people-manager
┣ 📂 backend (API .NET 8 + EF + Postgres)
┃ ┣ 📂 PeopleManager.API
┃ ┣ 📂 PeopleManager.Application
┃ ┣ 📂 PeopleManager.Infrastructure
┃ ┣ 📂 PeopleManager.Domain
┃ ┗ docker-compose.yml
┣ 📂 frontend (React + Vite + Tailwind)
┃ ┣ 📂 src
┃ ┣ 📜 vite.config.ts
┃ ┗ docker-compose.yml
┣ 📜 README.md
┗ 📜 .gitignore
```

---

## Funcionalidades

- **Autenticação com JWT**
- **RBAC**: níveis de acesso (`Employee < Leader < Director`)
- **CRUD completo de funcionários**
- **Validações:**
  - Nome e sobrenome obrigatórios
  - Email único e válido
  - Documento único
  - Pelo menos 2 telefones
  - Validação de maioridade (≥ 18 anos)
  - Senha forte
- **API documentada com Swagger**
- **Testes automatizados backend e frontend**
- **Ambientes Dev e Prod com Docker**

---

## Setup rápido com Docker

### Pré-requisitos
- Docker Desktop (Windows/Mac) ou Docker + Compose (Linux)  
- Portas livres: `8001` (API), `5173` (Frontend Dev), `8081` (Frontend Prod)

### 1. Backend (API + PostgreSQL)

```bash
cd backend
docker compose down -v   # limpa volumes antigos
docker compose up -d --build
```

- **Swagger:** http://localhost:8001/swagger
- **Health:** http://localhost:8001/health

**Admin padrão (seed):**
```
Email: admin@peoplemanager.com
Senha: Admin@12345
```

### 2a. Frontend (Dev, hot reload)

```bash
cd frontend/peoplemanager-frontend
export VITE_API_BASE_URL=http://localhost:8001   # Linux/Mac
set VITE_API_BASE_URL=http://localhost:8001      # Windows PowerShell

docker compose up -d fe-dev
```

**Acesse:** http://localhost:5173

### 2b. Frontend (Prod, Nginx)

```bash
cd frontend/peoplemanager-frontend
docker compose build fe-prod --build-arg VITE_API_BASE_URL=http://localhost:8001
docker compose up -d fe-prod
```

**Acesse:** http://localhost:8081

---

## Ambientes

- **Desenvolvimento:** Vite Dev Server + API .NET em modo Development
- **Produção:** Build do React servido via Nginx + API .NET otimizada
- **Banco de dados:** PostgreSQL 16 containerizado

---

## Autenticação e RBAC

- **JWT** gerado no login
- **Guardas de rota** no frontend
- **RBAC** aplicado no backend e frontend:
  - Employee não cria Leader/Director
  - Leader não cria Director
  - Director pode tudo

---

## Testes

### Backend
```bash
cd backend
dotnet test
```

- Validação de maioridade
- CRUD completo
- RBAC (não criar cargos acima)
- Login JWT

### Frontend
```bash
cd frontend/peoplemanager-frontend
npm test
```

- RBAC na interface
- Validação de idade
- Fluxo de login

---

## Documentação

**Swagger UI:** http://localhost:8001/swagger

**Endpoints principais:**
- `POST /api/v1/auth/login`
- `GET /api/v1/employees`
- `POST /api/v1/employees`
- `PUT /api/v1/employees/{id}`
- `DELETE /api/v1/employees/{id}`

---

## Estrutura de Pastas

### Backend
```
PeopleManager.API/Controllers      -> Controllers da API
PeopleManager.Application/Services -> Casos de uso
PeopleManager.Domain/Entities      -> Entidades de domínio
PeopleManager.Infrastructure       -> EF Core, Repositórios, Seed
```

### Frontend
```
src/components   -> Componentes reutilizáveis
src/pages        -> Páginas (Login, Employees CRUD)
src/context      -> AuthContext, RBAC
src/services     -> Axios + interceptors
src/tests        -> Vitest
```

---

## Screenshots

- Login
- Listagem de Funcionários
- Cadastro de Funcionário

---

## Contribuição

1. Faça um fork
2. Crie uma branch: `git checkout -b feature/minha-feature`
3. Commit suas mudanças: `git commit -m 'feat: adiciona minha feature'`
4. Push: `git push origin feature/minha-feature`
5. Abra um Pull Request

---

## Licença

Distribuído sob a licença MIT. Veja `LICENSE` para mais informações.

---

### References

1. **React, Node.js, Express and PostgreSQL CRUD app**. [https://www.corbado.com](https://www.corbado.com/blog/react-express-crud-app-postgresql)
2. **Heroku - Fazendo o Deploy de uma Web API containerizada**. [https://www.macoratti.net](https://www.macoratti.net/22/02/heroku_deployapi1.htm)
3. **Free REST API Tutorial - Criando APIs REST com .NET Core, EF, Autenticação e Heroku | Udemy**. [https://www.udemy.com](https://www.udemy.com/course/criando-apis-rest-com-net-core-ef-autenticacao-e-heroku/)
4. **Full Stack Web PRO do zero ao avançado! - Sujeito programador**. [https://sujeitoprogramador.com](https://sujeitoprogramador.com/fullstackpro/)
5. **GitHub - IgormBonfim/Projeto-CRUD-Gerenciamento-de-Funcionarios: Projeto CRUD de gerenciamento de funcionários, desenvolvido utilizando Angular e Spring, com o intuito de aprendizado**. [https://github.com](https://github.com/IgormBonfim/Projeto-CRUD-Gerenciamento-de-Funcionarios)
6. **Docker para Desenvolvedores .NET - Guia de Referência | by Renato Groffe | Medium**. [https://renatogroffe.medium.com](https://renatogroffe.medium.com/docker-para-desenvolvedores-net-guia-de-refer%C3%AAncia-6f9bad2c244e)
7. **Node.js: criando API Rest com autenticação, perfis de usuários e permissões | Alura Cursos Online**. [https://www.alura.com.br](https://www.alura.com.br/conteudo/node-js-api-rest-autenticacao-perfis-usuarios-permissoes)
8. **Construindo uma API com NestJS, PostgreSQL e Docker —Parte 6: Escrevendo testes | by Iago Maia Silva | Medium**. [https://medium.com](https://medium.com/@iago.maiasilva/construindo-uma-api-com-nestjs-postgresql-e-docker-parte-6-escrevendo-testes-ee23ca05f918)
9. **21 exemplos com implementações de APIs em ASP.NET Core 2.0 | by Renato Groffe | Medium**. [https://renatogroffe.medium.com](https://renatogroffe.medium.com/21-exemplos-com-implementa%C3%A7%C3%B5es-de-apis-em-asp-net-core-2-0-d3f44aa08811)
10. **[Akitando] #149 - Configurando Docker Compose, Postgres, com Testes de Carga - Parte Final da Rinha de Backend – AkitaOnRails.com**. [https://akitaonrails.com](https://akitaonrails.com/2023/12/16/akitando-149-configurando-docker-compose-postgres-com-testes-de-carga-parte-final-da-rinha-de-backend/)
