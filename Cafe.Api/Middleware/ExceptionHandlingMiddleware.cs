using Cafe.Application;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cafe.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                await WriteProblem(context, StatusCodes.Status400BadRequest, "Erro de validação",
                    ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            }
            catch (NotFoundException ex)
            {
                await WriteProblem(context, StatusCodes.Status404NotFound, "Não encontrado", [ex.Message]);
            }
            catch (ConflictException ex)
            {
                await WriteProblem(context, StatusCodes.Status409Conflict, "Conflito", [ex.Message]);
            }
                catch (UnauthorizedException ex)
            {
                await WriteProblem(context, StatusCodes.Status401Unauthorized, "Não autorizado", [ex.Message]);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex, "Conflito de concorrência");
                await WriteProblem(context, StatusCodes.Status409Conflict,
                    "Conflito de estoque",
                    ["O estoque mudou durante o processamento do pedido. Tente novamente."]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro não tratado");
                await WriteProblem(context, StatusCodes.Status500InternalServerError,
                    "Erro interno", ["Ocorreu um erro inesperado. Tente novamente."]);
            }
        }

        private static async Task WriteProblem(HttpContext context, int status, string title, IEnumerable<string> errors)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var problem = new
            {
                type = $"https://httpstatuses.io/{status}",
                title,
                status,
                errors,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
        }
    }
}