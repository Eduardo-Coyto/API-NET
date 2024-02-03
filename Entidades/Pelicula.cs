namespace APIPeli.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string? Poster { get; set; }    
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>(); // con esto configuro la relación uno a muchos. A pelicula le corresponde una lista de comentarios
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();
        public List<ActorPelicula> ActoresPeliculas { get; set; } = new List<ActorPelicula>();
    }
}
