# Documentação do Projeto WeatherData
Este código foi escrito apenas como uma demonstração simplória das habilidades de codificação, cujo objetivo é avaliar o grau de conhecimento técnico do candidato.
Por tanto, este código não implementa o estado-da-arte, nem em níveis de estrutura, codificação, ou melhores práticas

## Visão Geral
O projeto WeatherData é uma API para obter e gerenciar dados meteorológicos. Ele fornece endpoints para consultar informações meteorológicas de diferentes cidades e aeroportos, bem como funcionalidades CRUD (Create, Read, Update, Delete) para dados meteorológicos.

O ideal neste projeto é que o kubernetes inicie um script curl que:

1. Invoca as funcoes de previsão de tempo dos controladores do projeto

2. Invoca o POST do ODATA, passando os objetos de previsão para persistir em banco

Desta forma, a arquitetura estaria em conformidade as as boas práticas atuais.
Por motivos de brevidade, tal feito não foi entegue nesta demonstração.

## Estrutura de Pastas
O projeto segue uma estrutura de pastas organizada de acordo com princípios SOLID:

- **Controllers:** Contém os controladores da API, incluindo o `WeatherDataController` que lida com as operações CRUD.
- **DataAccess:** Contém o contexto do banco de dados e as migrações do Entity Framework Core.
- **Models:** Contém os modelos de dados utilizados na aplicação, incluindo `WeatherData`.
- **Services:** Contém a interface `IWeatherDataService` e a implementação `WeatherDataService` para recuperar dados meteorológicos da API BrasilAPI.

## Tecnologias Utilizadas
- OData: Framework para desenvolvimento de APIs RESTful.
- ASP.NET Core: Framework para desenvolvimento de APIs web.
- Entity Framework Core: Para acesso e gerenciamento de dados.
- Swagger: Utilizado para documentar a API e permitir testes interativos.
- Docker: Usado para empacotar a aplicação em contêineres.

## Configuração e Execução
Para configurar e executar o projeto, siga os passos abaixo:

1. Clone o repositório do projeto.

2. Certifique-se de ter o .NET Core SDK e o Docker instalados em sua máquina.

3. Configure a string de conexão com o banco de dados SQL Server no arquivo `appsettings.json` ou usando variáveis de ambiente.

4. Execute as migrações do Entity Framework Core para criar o banco de dados:

   ```
   dotnet ef database update
   ```
5. Execute o docker
 
6. Abra a aplicação

## Documentação da API
A API é documentada usando o Swagger, que fornece uma interface interativa para explorar e testar os endpoints da API. Acesse a documentação em `http://localhost:8080/swagger`.

## Testes
O projeto inclui testes de integraçao para garantir o funcionamento correto das funcionalidades. Os testes estão localizados na pasta `WeatherData.Tests` e podem ser executados usando um runner de testes compatível com xUnit.
Diversos testes estão falhando por um erro na migration, que ocorreu em última hora.

Também há alguns erros de ambiguidade de controllers devido ao Odata, que está tendo suas rotas confundidas com as rotas nativas do ASPnetCore.

Não houve mais tempo para reparar o problema.