
using APIPeli.DTOs;
using FluentValidation;

namespace APIPeli.Filtros
{
    public class FiltroValidacionesGeneros : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext contexto, EndpointFilterDelegate next)
        {
            var validador = contexto.HttpContext.RequestServices.GetService<IValidator<CrearGeneroDTO>>(); //es la manera de obtener un servicio con código

            if (validador is null)
            {
                return await next(contexto);
            }

            var insumoAValidar = contexto.Arguments.OfType<CrearGeneroDTO>().FirstOrDefault();

            if (insumoAValidar is null)
            {
                return TypedResults.Problem("No pudo ser encontrada la entidad a validar");
            }

            var resultadoValidacion = await validador.ValidateAsync(insumoAValidar);

            if (!resultadoValidacion.IsValid)
            {
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());
            }

            return await next(contexto);
        }
    }
}
