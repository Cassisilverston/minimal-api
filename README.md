# 🚀 Minimal API - Gestão de Veículos e Autenticação

> **Projeto de portfólio focado em arquitetura Back-End de alta performance, desenvolvido durante a formação .NET da DIO.**

Este projeto implementa uma **Minimal API** utilizando **.NET 10 (LTS)** e **C# 14**. O foco central é demonstrar domínio em organização arquitetural, Clean Code e documentação moderna para sistemas de gestão de frotas.

## 🛠️ Tecnologias e Ferramentas

* **Framework:** .NET 10 (LTS)
* **Linguagem:** C# 14
* **Banco de Dados:** MySQL 8.0 (com suporte a Migrations)
* **ORM:** Entity Framework Core
* **Documentação:** Microsoft.AspNetCore.OpenApi + Scalar API Reference
    * *Nota: Optei pelo Scalar por oferecer uma interface mais moderna e interativa que o Swagger convencional.*

## 🔭 Evolução Técnica (Upgrade de Versão)

Diferente da versão original proposta no curso (baseada em versões anteriores do .NET), este projeto foi **proativamente atualizado** por mim para as versões mais recentes:
* **Upgrade:** De .NET 6/7/8 para **.NET 10 (LTS)**.
* **Modernização:** Implementação de sintaxe e recursos do **C# 14**.
* **Documentação:** Migração do Swagger para o **Scalar API Reference** para uma interface de teste superior.

## 🌟 Funcionalidades Principais

- [x] **Gestão de Veículos:** CRUD completo para controle de frota.
- [x] **Autenticação de Administradores:** Sistema de login seguro.
- [x] **Paginação de Dados:** Listagem otimizada de administradores para performance.
- [x] **Validação de Inputs:** Camada de proteção com DTOs e mensagens de erro tratadas.

## 📐 Arquitetura da Solução

O projeto utiliza **Clean Architecture**, garantindo que as regras de negócio sejam independentes de frameworks e ferramentas externas.

### Estrutura de Pastas:
* **`Domain`**: Entidades (`Vehicle`, `Administrator`), DTOs, Interfaces de Serviço e `ModelViews`.
* **`Infrastructure`**: Contexto do banco de dados (`AppDbContext`) e persistência.
* **`Services`**: Implementação das regras de negócio e casos de uso.

```mermaid
graph TD
    User -->|Request| Endpoints{Minimal API}
    
    subgraph "Apresentação"
        Endpoints -->|JSON| Scalar[Scalar Documentation]
        Endpoints -->|Response| ModelViews[ModelViews]
    end

    subgraph "Core: Domain"
        Endpoints -->|DI| Services[Services: Vehicle/Admin]
        Services -->|Contratos| Interfaces[Interfaces]
        Services -->|Objetos| DTOs[LoginDTO / VehicleDTO]
    end

    subgraph "Infra: Data"
        Services -->|EF Core| DB[(MySQL 8.0)]
    end
