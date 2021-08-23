# Sobre o projeto

O teste é constituído por dois projetos:

- Server: Projeto em Asp.net Core trabalhando com WebSocket para a comunicação entre os usuários.
- Cliente: Frontend criado com Vuejs

O projeto permite comunicação em um canal, entre usuários e criação de canais.

# Para executar o projeto

Requisitos:
nodejs
.NET 5.0

- git clone https://github.com/hhendrikk/TakeChatTest.git

## Cliente:

- cd frontend
- npm install
- npm run serve

## Server:

- cd backend
- dotnet run --project Api/Api.csproj
