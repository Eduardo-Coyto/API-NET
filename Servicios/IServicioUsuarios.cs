using Microsoft.AspNetCore.Identity;

namespace APIPeli.Servicios
{
    public interface IServicioUsuarios
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}