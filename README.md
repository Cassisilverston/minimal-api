# 🚀 Minimal API - Gestão de Veículos e Autenticação

Este projeto é uma **Minimal API** desenvolvida em **.NET 10 (LTS)** e **C# 14**, focada em alta performance e escalabilidade. O sistema gerencia uma frota de veículos com controle de acesso rigoroso via **JWT** e perfis diferenciados (**Adm/Editor**).

## 📐 Arquitetura da Solução

Utilizamos uma abordagem de **Clean Architecture** para separar as responsabilidades de negócio da persistência de dados no **MySQL 8.0**.

```mermaid
graph TD
    User((Usuário)) -->|Login/Senha| API[Minimal API - Auth]
    API -->|Valida| DB[(MySQL 8.0)]
    API -->|Gera Token JWT| User
    
    User -->|Token + Requests| Routes{Endpoints Veículos}
    Routes -->|POST/PUT/DELETE| Admin[Regra: Administrador]
    Routes -->|GET| Public[Lista Paginada]
    
    subgraph "Core Business"
    Admin
    Public
    end
