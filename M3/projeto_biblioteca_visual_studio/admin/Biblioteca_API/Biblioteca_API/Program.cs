using Biblioteca;
using Biblioteca.Dt;
using Biblioteca.Model;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsProfile",
    policy =>
    {
        policy
        .WithOrigins("http://127.0.0.1:5500/", "https://localhost:44351/", "http://admin.test/")
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
}
);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("CorsProfile");

app.UseHttpsRedirection();

app.MapPost("/Admin_RegistrarNovoUtilizador", (string nome, DateTime dataNascimento, string email, string telefone, string username, string palavraPasse, string tipoUser) =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    return obj.Admin_RegistrarNovoUtilizador(nome, dataNascimento, email, telefone, username, palavraPasse, tipoUser);
})
.WithName("Admin_RegistrarNovoUtilizador")
.WithOpenApi();

app.MapGet("/Admin_ListarLeitoresInativos", () =>
{
    try
    {
        Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
        List<Usuario> leitoresInativos = obj.Admin_ListarLeitoresInativos();

        return Results.Ok(leitoresInativos);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: "Erro ao listar leitores inativos: " + ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("Admin_ListarLeitoresInativos")
.WithOpenApi();

app.MapPost("/Admin_reativarLeitor", async ([FromBody] int leitorId) =>
{
    try
    {
        Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
        var resultado = obj.Admin_reativarLeitor(leitorId);

        return Results.Json(resultado);
    }
    catch (Exception ex)
    {
        return Results.Json(
            new { error = ex.Message, stackTrace = ex.StackTrace },
            statusCode: StatusCodes.Status400BadRequest
        );
    }
})
.WithName("Admin_reativarLeitor")
.WithOpenApi();

app.MapPost("/Admin_suspenderAcessoLeitoresDevolucoesAtrasadas", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    return obj.Admin_suspenderAcessoLeitoresDevolucoesAtrasadas();
})
.WithName("Admin_suspenderAcessoLeitoresDevolucoesAtrasadas")
.WithOpenApi();

app.MapPost("/Admin_EliminarLeitoresInativos", () =>
{
    try
    {
        Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
        List<Usuario> leitoresRemovidos = obj.EliminarLeitoresInativos();

        return Results.Ok(leitoresRemovidos);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: "Erro ao eliminar leitores inativos: " + ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("Admin_EliminarLeitoresInativos")
.WithOpenApi();

app.MapGet("/MostrarTodasObras", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    return obj.MostrarTodasObras();
})
.WithName("MostrarTodasObras")
.WithOpenApi();

app.MapGet("/ConsultarTotalObras", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    return obj.ConsultarTotalObras();
})
.WithName("ConsultarTotalObras")
.WithOpenApi();

app.MapGet("/ConsultarObrasPorGenero", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    return obj.ConsultarObrasPorGenero();
})
.WithName("ConsultarObrasPorGenero")
.WithOpenApi();

app.MapGet("/ObterObras", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    var resultado = obj.ObterObras();

    return Results.Json(resultado);
})
.WithName("ObterObras")
.WithOpenApi();

app.MapGet("/ObterNucleos", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    var resultado = obj.ObterNucleos();

    return Results.Json(resultado);
})
.WithName("ObterNucleos")
.WithOpenApi();

app.MapGet("/ObterExemplaresPorNucleo", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    var resultado = obj.ObterExemplaresPorNucleo();

    return Results.Json(resultado);
})
.WithName("ObterExemplaresPorNucleo")
.WithOpenApi();

app.MapPost("/AdicionarObra", async (HttpContext context) =>
{
    try
    {
        var obraDto = await context.Request.ReadFromJsonAsync<ObraAtualizacaoDto>();
        if (obraDto == null)
        {
            return Results.BadRequest("Dados da obra inválidos.");
        }

        byte[] imagemBytes = null;
        if (!string.IsNullOrEmpty(obraDto.ImagemBase64))
        {
            try
            {
                var base64Data = obraDto.ImagemBase64.Split(',')[1];
                imagemBytes = Convert.FromBase64String(base64Data);
            }
            catch
            {
                return Results.BadRequest("Formato de imagem inválido.");
            }
        }

        var biblioteca = new Biblioteca.Biblioteca(connectionString);
        biblioteca.AdicionarObra(obraDto, imagemBytes);
        return Results.Ok("Obra adicionada com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
        return Results.Problem($"Erro ao adicionar obra: {ex.Message}");
    }
})
.WithName("AdicionarObra")
.WithOpenApi()
.Accepts<ObraAtualizacaoDto>("application/json");

app.MapPost("/ApagarObra", (int obraID) =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    string mensagem = obj.ApagarObra(obraID);
    return Results.Ok(mensagem);
})
.WithName("ApagarObra")
.WithOpenApi();

app.MapPost("/ObterDadosObra", (int obraID) =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    var obra = obj.ObterDadosObra(obraID);
    return Results.Json(obra);
})
.WithName("ObterDadosObra")
.WithOpenApi();

app.MapPost("/AtualizarObra", (ObraAtualizacaoDto obraDto) =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    obj.AtualizarObra(obraDto);
    return Results.Ok("Obra atualizada com sucesso!");
})
.WithName("AtualizarObra")
.WithOpenApi();

app.MapPost("/AdicionarExemplaresAoNucleoPrincipal", (int obraID, int qtdAdicionar) =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    obj.AdicionarExemplaresAoNucleoPrincipal(obraID, qtdAdicionar);
})
.WithName("AdicionarExemplaresAoNucleoPrincipal")
.WithOpenApi();

app.MapPost("/Admin_TransferirExemplar", (int obraId, int origemNucleoId, int destinoNucleoId, int qtdTransferir) =>
{
        Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
        var resultado = obj.Admin_TransferirExemplar(obraId, origemNucleoId, destinoNucleoId, qtdTransferir);
        return Results.Json(resultado);
})
.WithName("Admin_TransferirExemplar")
.WithOpenApi();

app.MapGet("/Admin_Top10ObrasRequisitadasUltimoAno", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);

    try
    {
        var topObras = obj.Admin_Top10ObrasRequisitadasUltimoAno();

        return Results.Json(new
        {
            sucesso = true,
            obras = topObras.Select(o => new
            {
                Titulo = o.Titulo,
                TotalRequisicoes = o.TotalRequisicoes
            })
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            sucesso = false,
            mensagem = "Erro ao buscar as top 10 obras requisitadas no último ano",
            detalhes = ex.Message
        });
    }
})
.WithName("Admin_Top10ObrasRequisitadasUltimoAno")
.WithOpenApi();

app.MapGet("/Admin_ObterUltimaRequisicaoPorNucleo", () =>
{
    Biblioteca.Biblioteca obj = new Biblioteca.Biblioteca(connectionString);
    var resultado = obj.Admin_ObterUltimaRequisicaoPorNucleo();

    return Results.Json(resultado);
})
.WithName("Admin_ObterUltimaRequisicaoPorNucleo")
.WithOpenApi();

app.Run();