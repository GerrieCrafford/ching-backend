<h1 align="center">
  <img alt="ching logo" src="https://github.com/GerrieCrafford/ching-backend/blob/main/.github/ching-logo.png" width="96px"/><br/> Ching Backend
</h1>
<p align="center">
Backend for Ching, my own personal finance management web app.
</p>

## Overview

The Ching backend, together with the frontend, allows me to keep track of my finances by tracking expenses and assigning transactions to budget categories.

This repo is a WIP, as I am currently doing a complete rewrite of the first version and using it as an opportunity to learn the below patterns.

## Patterns used in the Ching backend

- Vertical slice architecture
- CQRS and MediatR
- Fluent Validation
- EF core
- Automapper
- .NET minimal API

## Usage

Install dependencies with:

```
$ dotnet restore
```

Build and run with:

```
$ dotnet run --project Ching
```

Run tests with:

```
$ dotnet test
```

Migrate database with:

```
$ dotnet ef database update --context ChingContext
```
