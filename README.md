# 🚀 Minimal API - Gestão de Veículos e Autenticação

> **Projeto de portfólio focado em arquitetura Back-End de alta performance, desenvolvido durante a formação .NET da DIO.**

Este projeto implementa uma **Minimal API** utilizando **.NET 10 (LTS)** e **C# 14**. O foco central é demonstrar domínio em organização arquitetural, Clean Code e segurança robusta para sistemas de gestão de frotas.

## 🛠️ Tecnologias e Ferramentas

* **Framework:** .NET 10 (LTS)
* **Linguagem:** C# 14
* **Banco de Dados:** MySQL 8.0
* **Autenticação:** JWT (JSON Web Token) com suporte a Bearer Token.
* **ORM:** Entity Framework Core
* **Documentação:** Microsoft.AspNetCore.OpenApi + Scalar API Reference
    * *Nota: O Scalar foi configurado customizadamente para permitir a passagem do Token JWT diretamente pela interface.*

## 🔭 Evolução Técnica (Upgrade de Versão)

Diferente da versão original proposta no curso, este projeto foi **proativamente atualizado** por mim:
* **Upgrade:** De .NET 6/7/8 para **.NET 10 (LTS)**.
* **Modernização:** Uso intensivo de novos recursos do **C# 14**.
* **Documentação:** Migração estratégica do Swagger para o **Scalar**.

## 🛡️ Segurança e Autorização (RBAC)

O sistema conta com uma camada de segurança avançada que diferencia as permissões de acesso:
- **Perfil ADM:** Acesso total ao sistema, incluindo criação e deleção.
- **Perfil Editor:** Permissão para visualização e edições específicas.
- **Middleware:** Validação de tokens JWT em todas as rotas sensíveis.

## 🌟 Funcionalidades Principais

- [x] **Gestão de Veículos:** CRUD completo para controle de frota.
- [x] **Autenticação Segura:** Geração de token JWT para administradores logados.
- [x] **Controle de Perfis:** Autorização granular (Adm/Editor).
- [x] **Paginação de Dados:** Listagem otimizada para performance.
- [x] **Validação de Inputs:** Proteção com DTOs e mensagens de erro tratadas.

## 📐 Arquitetura da Solução

O projeto utiliza **Clean Architecture**, garantindo que as regras de negócio sejam independentes de frameworks externos.

### Estrutura de Pastas:
* **`Domain`**: Entidades, DTOs, Enums, Interfaces e `ModelViews`.
* **`Infrastructure`**: Contexto do banco de dados (`AppDbContext`) e persistência.
* **`Services`**: Implementação das regras de negócio e lógica de autenticação.

```mermaid
graph TD
    User -->|Request| Endpoints{Minimal API}
    
    subgraph "Apresentação"
        Endpoints -->|JSON| Scalar[Scalar Documentation + Auth]
        Endpoints -->|Response| ModelViews[ModelViews / AdminLogged]
    end

    subgraph "Core: Domain"
        Endpoints -->|DI| Services[Services: Vehicle/Admin]
        Services -->|Contratos| Interfaces[Interfaces]
        Services -->|Objetos| DTOs[LoginDTO / AdminDTO]
    end

    subgraph "Infra: Data"
        Services -->|EF Core| DB[(MySQL 8.0)]
    end
