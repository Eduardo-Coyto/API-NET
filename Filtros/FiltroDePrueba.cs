
using APIPeli.Repositorios;
using AutoMapper;

namespace APIPeli.Filtros
{
    public class FiltroDePrueba : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext contexto, EndpointFilterDelegate next)
        {
            // Este código se ejecuta antes del endpoint

            var paramRepositorioGeneros = contexto.Arguments.OfType<IRepositorioGeneros>().FirstOrDefault();
            var paramEntero = contexto.Arguments.OfType<int>().FirstOrDefault();
            var paramMapper = contexto.Arguments.OfType<IMapper>().FirstOrDefault();

            var resultado = await next(contexto);
            // Este código se ejecuta después del endpoint
            return resultado;
        }
    }
}
