using Microsoft.EntityFrameworkCore;

namespace APIPeli.Servicios
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPAginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if(httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Append("cantidadTotalRegistros", cantidad.ToString()); 
        }
    }
}
