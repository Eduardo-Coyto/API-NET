using Microsoft.AspNetCore.Identity;

namespace APIPeli.Servicios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;

        public ServicioUsuarios(
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<IdentityUser?> ObtenerUsuario()
        {
            // a través de httpContextAccessor accedo a HttpContext. A través de ese contexto puedo obtener el usuario que esta logueado.
            var emailClaim = httpContextAccessor.HttpContext!
                    .User.Claims.Where(x => x.Type == "email").FirstOrDefault();

            if (emailClaim is null)
            {
                return null;
            }

            var email = emailClaim.Value;
            // con userManager obtengo al usuario
            return await userManager.FindByEmailAsync(email);
        }
    }
}
