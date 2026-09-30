using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SetupVault.Api.Exceptions;

namespace SetupVault.Api.Middlewares;

/// <summary>
/// Converte exceções em respostas padronizadas no formato ProblemDetails (RFC 9457),
/// evitando try/catch espalhado pelos controllers.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            BusinessRuleException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado ao processar {Metodo} {Rota}",
                httpContext.Request.Method, httpContext.Request.Path);
        else
            logger.LogWarning("{Titulo}: {Mensagem}", titulo, exception.Message);

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                // Nunca expõe detalhes internos em erro 500
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                    : exception.Message,
                Instance = httpContext.Request.Path
            }
        });
    }
}
