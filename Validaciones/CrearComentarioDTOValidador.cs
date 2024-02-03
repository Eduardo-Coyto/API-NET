using APIPeli.DTOs;
using FluentValidation;

namespace APIPeli.Validaciones
{
    public class CrearComentarioDTOValidador : AbstractValidator<CrearComentarioDTO>
    {
        public CrearComentarioDTOValidador()
        {
            RuleFor(x => x.Cuerpo).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);
        }
    }
}
