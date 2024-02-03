using APINETEF.Entidades;

namespace APIPeli.Entidades
{
    public class GeneroPelicula
    {
        public int PeliculaId { get; set; }
        public int GeneroId { get; set; }
        public Genero Genero { get; set; } = null!; //estas propiedades para EF Core son utilidades y no seran representadas en la tabla de la BD. Si miramos la Migración realizada se puede ver con claridad
        public Pelicula Pelicula { get; set; } = null!; //estas propiedades para EF Core son utilidades y no seran representadas en la tabla de la BD. Si miramos la Migración realizada se puede ver con claridad
    }
}
