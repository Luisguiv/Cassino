# 🎰 API Cassino - Sistema de Apostas Online

Sistema completo de apostas online desenvolvido em **.NET 6** com **Entity Framework Core**, **MySQL** e arquitetura baseada em **Clean Architecture** com **Repository Pattern** e testes unitários abrangentes.

![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=for-the-badge&logo=dotnet)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Tests](https://img.shields.io/badge/Tests-20%20Passing-brightgreen?style=for-the-badge)

---

## 📋 Índice

- [✨ Características](#-características)
- [🏗️ Arquitetura](#️-arquitetura)
- [📁 Estrutura do Projeto](#-estrutura-do-projeto)
- [🗄️ Modelo Entidade Relacional (MER)](#️-modelo-entidade-relacional-mer)
- [🚀 Como Executar](#-como-executar)
- [🧪 Testes](#-testes)
- [📡 API REST - Endpoints](#-api-rest---endpoints)
- [🎯 Regras de Negócio](#-regras-de-negócio)
- [✅ Validações](#-validações)
- [🔄 Padrão Repository](#-padrão-repository)
- [📊 Controle de Migrations](#-controle-de-migrations)
- [🛠️ Tecnologias](#️-tecnologias)
- [🔗 Links Úteis](#-links-úteis)

---

## ✨ Características

- 🎲 **Sistema de apostas** com 30% de chance de vitória
- 💰 **Saldo inicial** de R$ 1.000 para novos jogadores
- 🎁 **Sistema de bônus** por 5 apostas perdidas consecutivas
- ↩️ **Cancelamento flexível** de apostas (ativa, ganha ou perdida)
- 📈 **Histórico completo** de transações paginado
- 🧪 **20 testes unitários** com 100% de aprovação
- 🏛️ **Clean Architecture** com Repository Pattern
- 🐳 **Docker** para ambiente de desenvolvimento
- 📝 **Documentação completa** com Swagger

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture** e **SOLID**, utilizando:
<pre>
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Controllers   │───▶│    Services     │───▶│  Repositories   │
│  (API Layer)    │    │ (Business Logic)│    │  (Data Access)  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                        │                        │
         ▼                        ▼                        ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│      DTOs       │    │   Interfaces    │    │  Entity Models  │
│ (Data Transfer) │    │ (Abstractions)  │    │  (Domain)       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
</pre>
