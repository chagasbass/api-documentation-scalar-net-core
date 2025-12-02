## Documentação da Extensão Scalar para Minimal APIs ASP.NET Core

### 📋 Visão Geral
Esta extensão proporciona uma solução completa para documentação e versionamento de APIs em projetos ASP.NET Core Minimal APIs, utilizando Scalar como interface de usuário e OpenAPI como padrão de especificação.

---

#### Funcionalidades Principais:

- Configuração centralizada de documentação via appsettings.json
- Versionamento automático de endpoints (v1, v2, etc.)
- Interface interativa Scalar para exploração de APIs
- Personalização de metadados da API
- Suporte a autenticação JWT (Bearer Token)
- Transformação dinâmica de documentos OpenAPI

### 🎏 Dependências 

Pacotes NuGet Necessários
xml
<PackageReference Include="Scalar.AspNetCore" Version="2.11.0" />
<PackageReference Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.1.0" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />

``` shell
dotnet add package Scalar.AspNetCore
dotnet add package Asp.Versioning.Mvc.ApiExplorer
dotnet add package Microsoft.AspNetCore.OpenApi

```
---

### 🏗️ Estrutura do Projeto

1. Classe Principal de Configuração

``` csharp
public class DocumentationConfigurationOptions
```
****Propósito****: Armazena as configurações de documentação lidas do appsettings.json.

**Propriedades**:

- NomeAplicacao: Nome da aplicação exibido na documentação
- Desenvolvedor: Nome do desenvolvedor/contato
- Descricao: Descrição detalhada da API
- RotaDocumentacao: Rota personalizada para acessar a UI Scalar
- TemAutenticacao: Define se a API requer autenticação

#### Configuração no appsettings.json:

``` csharp
{
  "DocumentationConfiguration": {
    "NomeAplicacao": "Automação de Processos API",
    "Desenvolvedor": "Equipe de Desenvolvimento Veste",
    "Descricao": "API para automação de processos",
    "RotaDocumentacao": "/docs",
    "TemAutenticacao": true
  }
}

```

2. Transformador de Metadados da API

``` csharp
public class ApiMetadataTransformer : IOpenApiDocumentTransformer
``` 

**Propósito**: Intercepta e personaliza o documento OpenAPI antes da renderização.

#### Funcionalidades:

- Define título dinâmico com sufixo de versão
- Configura versão da documentação
- Adiciona informações de contato
- Aplica descrições personalizadas

Exemplo de uso automático: A classe é registrada automaticamente durante a configuração do OpenAPI.

3. Extensões de Documentação

``` csharp
public static class DocumentationExtensions
```

### Métodos Principais:

``` csharp
AddDocumentationVersioningConfig()

public static IServiceCollection AddDocumentationVersioningConfig(
    this IServiceCollection services, 
    List<string> versions)

```

**Propósito**: Configura o versionamento da API e registra as versões do OpenAPI.

#### Parâmetros:

- **services**: Coleção de serviços do DI
- **versions**: Lista de versões suportadas (ex: ["v1", "v2"])

#### Configurações realizadas:

- **API Versioning**: Configura versionamento padrão e relatório de versões
- **API Explorer**: Formata nomes de grupos e substituição de versão na URL
- **OpenAPI por versão**: Registra documentos OpenAPI para cada versão

<br>

``` csharp
UseScalarDocumentation()

public static IApplicationBuilder UseScalarDocumentation(this WebApplication app)

```

****Propósito****: Configura e expõe a interface Scalar para documentação.

#### Características:

- Rota configurável via RotaDocumentacao
- Tema padrão Scalar com sidebar ativada
- Suporte a download de documentação (JSON/YAML)
- Configuração automática de segurança quando TemAutenticacao = true
- Ocultação de clients quando não há autenticação

4. Extensões de Versionamento

``` csharp
public static class VersionExtensions
``` 

#### Método Principal:

``` csharp
RetornarVersaoDeEndpoints()

public static ApiVersionSet RetornarVersaoDeEndpoints(WebApplication app)

```

****Propósito****: Cria e configura o conjunto de versões suportadas pela API.

##### Versões configuradas:

- v1.0
- v2.0

5. Extensões de Configuração

``` csharp
public static class OptionsExtensions
```

Método Principal:

``` csharp
ConfigureDocumentationOptions()

public static IServiceCollection ConfigureDocumentationOptions(
    this IServiceCollection services, 
    IConfiguration configuration)

```

****Propósito****: Registra a configuração de documentação no sistema de injeção de dependências.

6. Extensões de Endpoints (Exemplo)

****Propósito****: Demonstração de implementação de endpoints versionados.

``` csharp
public static class WeathersEndpointExtensions
``` 

#### Método Principal:

``` csharp
InicializarEndpoints()

public static void InicializarEndpoints(this WebApplication app)
```

#### Exemplo de uso:

``` csharp
public static class WeathersEndpointExtensions
{
    static List<string> summaries = new List<string>
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
    public static void InicializarEndpoints(this WebApplication app)
    {
        var apiVersionSet = VersionExtensions.RetornarVersaoDeEndpoints(app);

        app.MapGet("/v{version:apiVersion}/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Count)]
                ))
                .ToImmutableList();
            return forecast;
        })
           .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized, typeof(ProblemDetails))
           .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
           .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
           .Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
           .WithName("WeathersV1")
           .WithTags("WeathersV1")
           .WithDescription("Endpoint resposável por exibir a previsão do tempo versão 1.")
           .WithSummary("Endpoint de Teste de aplicação V1")
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new ApiVersion(1, 0));

        app.MapGet("/v{version:apiVersion}/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Count)]
                ))
                .ToImmutableList();
            return forecast;
        })
         .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
         .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
         .Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
         .WithName("WeathersV2")
         .WithTags("WeathersV2")
         .WithDescription("Endpoint resposável por exibir a previsão do tempo versão 2.(Precisa de autenticação)")
         .WithSummary("Endpoint de Teste de aplicação V2")
         .WithApiVersionSet(apiVersionSet)
         .MapToApiVersion(new ApiVersion(2, 0))
         .RequireAuthorization();
    }
}

```

## 🚀 Implementação no Projeto

Passo 1: Configuração do Program.cs

``` csharp
var builder = WebApplication.CreateBuilder(args);
``` 

Defina as versões suportadas

``` csharp
var versions = new List<string> { "v1", "v2" };
```

Configure os serviços

``` csharp
builder.Services.ConfigureDocumentationOptions(builder.Configuration);
builder.Services.AddDocumentationVersioningConfig(builder.Configuration, versions);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

```

Configure o pipeline de Middleware (A Ordem deve ser respeitada)

``` csharp

app.MapOpenApi();
app.UseScalarDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

```

Inicialize os endpoints antes do app.Run()

``` csharp

WeathersEndpointExtensions.InicializarEndpoints(app);

app.Run();

```

#### Passo 2: Configuração do appsettings.json

- Aqui colocamos as informações necessárias para preenchimento do arquivo open api gerado da api 
e dados mostrados na UI do Scalar.Caso as propriedades não sejam preenchidas, textos genéricos serão inseridos.

<br>

``` csharp
{
  "DocumentationConfiguration": {
    "NomeAplicacao": "Veste API",
    "Desenvolvedor": "Equipe de Integração Veste",
    "Descricao": "API para otimização e automação de processos Veste, integrando ERP, CRM e plataformas de e-commerce.",
    "RotaDocumentacao": "/api-docs",
    "TemAutenticacao": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}

```

#### Passo 3: Configuração de Autenticação (Opcional)

para a configuração de autenticação, irá existir uma Extension criada para isso.

---

<br>

📊 Benefícios para o Projeto

- **Documentação Centralizada**: Todos os endpoints versionados em uma interface única
- **Facilidade de Testes**: Interface Scalar permite testar endpoints diretamente
- **Versionamento Controlado**: Transição suave entre versões da API
- **Segurança**: Integração com autenticação Azure AD para clientes B2B
- **Manutenibilidade**: Configuração centralizada e extensível

### 🔧 Configurações Avançadas

**Personalização do Scalar**

(https://guides.scalar.com/scalar/scalar-api-references/integrations/net-aspnet-core/integration)

**Adicionando Novas Versões de Endpoints**

``` csharp
// No Program.cs
var versions = new List<string> { "v1", "v2", "v3" };

// No VersionExtensions
public static ApiVersionSet RetornarVersaoDeEndpoints(WebApplication app)
{
    return app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1, 0))
        .HasApiVersion(new ApiVersion(2, 0))
        .HasApiVersion(new ApiVersion(3, 0)) // Nova versão
        .ReportApiVersions()
        .Build();
}
```

### 📝 Boas Práticas

1. Nomeclatura de Endpoints
**text
/v{version}/[recurso]/[ação]**
Exemplo: **/v1/pedidos/processar**

2. Documentação de Endpoints

- Use **WithDescription()** para explicar a funcionalidade
- Use **WithSummary()** para um resumo breve
- Especifique códigos de resposta com **Produces()**

3. Versionamento

- Mantenha compatibilidade com versões anteriores
- Documente breaking changes claramente
- Use feature flags para novas #### Funcionalidades


### 🔗 Recursos Adicionais

**Referências**:

- [Documentação Oficial Scalar](https://guides.scalar.com/scalar/scalar-api-references/integrations/net-aspnet-core/integration)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0)
- [OpenAPI Specification](https://spec.openapis.org/oas/v3.2.0.html)
