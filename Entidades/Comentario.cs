using Microsoft.AspNetCore.Identity;

namespace APIPeli.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Cuerpo { get; set; } = null!;
        public int PeliculaId { get; set; }// con esto configuro la relacion entre pelicula y comentario. a cada comentario le corresponde una unica pelicula
        public string UsuarioId { get; set; } = null!;
        public IdentityUser Usuario { get; set; } = null!;
    }
}
