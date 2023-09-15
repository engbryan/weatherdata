# Documenta��o do Projeto WeatherData
Este c�digo foi escrito apenas como uma demonstra��o simpl�ria das habilidades de codifica��o, cujo objetivo � avaliar o grau de conhecimento t�cnico do candidato.
Por tanto, este c�digo n�o implementa o estado-da-arte, nem em n�veis de estrutura, codifica��o, ou melhores pr�ticas

## Vis�o Geral
O projeto WeatherData � uma API para obter e gerenciar dados meteorol�gicos. Ele fornece endpoints para consultar informa��es meteorol�gicas de diferentes cidades e aeroportos, bem como funcionalidades CRUD (Create, Read, Update, Delete) para dados meteorol�gicos.

O ideal neste projeto � que o kubernetes inicie um script curl que:

1. Invoca as funcoes de previs�o de tempo dos controladores do projeto

2. Invoca o POST do ODATA, passando os objetos de previs�o para persistir em banco

Desta forma, a arquitetura estaria em conformidade as as boas pr�ticas atuais.
Por motivos de brevidade, tal feito n�o foi entegue nesta demonstra��o.

## Estrutura de Pastas
O projeto segue uma estrutura de pastas organizada de acordo com princ�pios SOLID:

- **Controllers:** Cont�m os controladores da API, incluindo o `WeatherDataController` que lida com as opera��es CRUD.
- **DataAccess:** Cont�m o contexto do banco de dados e as migra��es do Entity Framework Core.
- **Models:** Cont�m os modelos de dados utilizados na aplica��o, incluindo `WeatherData`.
- **Services:** Cont�m a interface `IWeatherDataService` e a implementa��o `WeatherDataService` para recuperar dados meteorol�gicos da API BrasilAPI.

## Tecnologias Utilizadas
- OData: Framework para desenvolvimento de APIs RESTful.
- ASP.NET Core: Framework para desenvolvimento de APIs web.
- Entity Framework Core: Para acesso e gerenciamento de dados.
- Swagger: Utilizado para documentar a API e permitir testes interativos.
- Docker: Usado para empacotar a aplica��o em cont�ineres.

## Configura��o e Execu��o
Para configurar e executar o projeto, siga os passos abaixo:

1. Clone o reposit�rio do projeto.

2. Certifique-se de ter o .NET Core SDK e o Docker instalados em sua m�quina.

3. Configure a string de conex�o com o banco de dados SQL Server no arquivo `appsettings.json` ou usando vari�veis de ambiente.

4. Execute as migra��es do Entity Framework Core para criar o banco de dados:

   ```
   dotnet ef database update
   ```
5. Execute o docker
 
6. Abra a aplica��o

## Documenta��o da API
A API � documentada usando o Swagger, que fornece uma interface interativa para explorar e testar os endpoints da API. Acesse a documenta��o em `http://localhost:8080/swagger`.

## Testes
O projeto inclui testes de integra�ao para garantir o funcionamento correto das funcionalidades. Os testes est�o localizados na pasta `WeatherData.Tests` e podem ser executados usando um runner de testes compat�vel com xUnit.
Diversos testes est�o falhando por um erro na migration, que ocorreu em �ltima hora.

Tamb�m h� alguns erros de ambiguidade de controllers devido ao Odata, que est� tendo suas rotas confundidas com as rotas nativas do ASPnetCore.

N�o houve mais tempo para reparar o problema.