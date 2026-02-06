# 🚀 Minimal API - Gestão de Veículos e Autenticação

Este projeto é uma **Minimal API** desenvolvida em **.NET 10 (LTS)** e **C# 14**, focada em alta performance e organização arquitetural. O sistema gerencia uma frota de veículos com autenticação e documentação moderna.

## 🛠️ Tecnologias e Ferramentas

* **Framework:** .NET 10 (LTS)
* **Linguagem:** C# 14
* **Banco de Dados:** MySQL 8.0
* **ORM:** Entity Framework Core
* **Documentação:** Microsoft.AspNetCore.OpenApi + Scalar API Reference (Como já tenho domínio do Swagger, resolvi explorar novas alterantivas)

## 📐 Arquitetura da Solução

O projeto utiliza **Clean Architecture**, separando as preocupações de infraestrutura da lógica de negócio central.

### Estrutura de Pastas:
* **Domain**: Contém as Entidades (`Vehicle`, `Administrator`), DTOs, Interfaces de Serviço e `ModelViews` (estruturas leves para respostas da API).
* **Infrastructure**: Gerencia o contexto do banco de dados (`AppDbContext`) e a persistência.
* **Services**: Implementação das regras de negócio para Veículos e Administradores.

```mermaid
graph TD
    User -->|Request| Endpoints{Minimal API}
    
    subgraph "Camada de Apresentação"
        Endpoints -->|JSON| Scalar[Scalar API Documentation]
        Endpoints -->|Response| ModelViews[ModelViews / Home Struct]
    end

    subgraph "Core: Domain"
        Endpoints -->|DI| Services[Services: Vehicle/Admin]
        Services -->|Contratos| Interfaces[Interfaces: IService]
        Services -->|Data Objects| DTOs[LoginDTO / VehicleDTO]
    end

    subgraph "Infra: Data"
        Services -->|EF Core| DB[(MySQL 8.0)]
    end
