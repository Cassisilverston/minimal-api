# 🚀 Minimal API - Gestão de Veículos e Autenticação

> **Projeto de portfólio focado em arquitetura Back-End de alta performance, desenvolvido durante a formação .NET da DIO.**

Este projeto implementa uma **Minimal API** utilizando **.NET 10 (LTS)** e **C# 14**. O foco central é demonstrar domínio em organização arquitetural, Clean Code, segurança e qualidade de software com testes de integração automatizados.

## 🛠️ Tecnologias e Ferramentas

* **Framework:** .NET 10 (LTS)
* **Linguagem:** C# 14
* **Banco de Dados:** MySQL 8.0
* **Autenticação:** JWT (JSON Web Token) com suporte a Bearer Token
* **ORM:** Entity Framework Core
* **Documentação:** Microsoft.AspNetCore.OpenApi + Scalar API Reference

## 🔭 Evolução Técnica (Upgrade de Versão)

Diferente da versão original proposta no curso, este projeto foi **proativamente atualizado** por mim para as tecnologias mais recentes do mercado:
* **Upgrade:** Migração de versões legadas para **.NET 10 (LTS)**.
* **Modernização:** Uso intensivo de novos recursos de sintaxe do **C# 14**.
* **Documentação:** Substituição do Swagger pelo **Scalar API Reference**, configurado para suportar autenticação JWT diretamente na interface.

## 🛡️ Segurança e Melhores Práticas (DevOps)

Para garantir a integridade e segurança do projeto, apliquei padrões profissionais de desenvolvimento:
* **Secrets Management:** Implementação de **.NET User Secrets** para isolar credenciais sensíveis, garantindo que chaves de banco de dados nunca sejam expostas no GitHub.
* **Injeção de Dependência:** Refatoração do `AppDbContext` para suporte a **Inversão de Controle (IoC)**, permitindo o isolamento total entre ambientes de desenvolvimento e teste.
* **RBAC (Role-Based Access Control):** Controle de acesso granular diferenciando permissões entre perfis **ADM** e **Editor**.

## 🧪 Qualidade de Software (QA & Testes)

O projeto conta com uma suíte de testes de integração robusta para validar as regras de negócio:
* **Tecnologias:** MSTest e Entity Framework InMemory/Test Database.
* **Cobertura:** Ciclo de vida completo do `AdministratorService` (Cadastro, Busca, Listagem e Autenticação).
* **Concorrência:** Controle de paralelismo (`[DoNotParallelize]`) para garantir a integridade dos dados durante operações de limpeza de banco (`TRUNCATE`) entre execuções.

## 📐 Arquitetura da Solução

O projeto segue uma estrutura de **Solution-Based Architecture**, organizada para facilitar a escalabilidade:

### Estrutura de Pastas:
* **`Api/`**: Código de produção (Endpoints, Domain, Services e Infrastructure).
* **`Test/`**: Suíte de testes de integração e configurações de ambiente de QA.

```mermaid
graph TD
    User -->|Request| Endpoints{Minimal API}
    
    subgraph "Apresentação (Api/)"
        Endpoints -->|JSON| Scalar[Scalar Documentation + Auth]
        Endpoints -->|Response| ModelViews[ModelViews / AdminLogged]
    end

    subgraph "Core: Domain (Api/Domain/)"
        Endpoints -->|DI| Services[Services: Vehicle/Admin]
        Services -->|Contratos| Interfaces[Interfaces]
        Services -->|Objetos| DTOs[LoginDTO / AdminDTO]
    end

    subgraph "Infra & QA"
        Services -->|EF Core| DB[(MySQL 8.0)]
        TestProject[Test Project] -->|Integration| Services
    end
