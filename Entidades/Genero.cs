using APIPeli.Entidades;
using System.ComponentModel.DataAnnotations;

namespace APINETEF.Entidades
{
    public class Genero
    {
        /*
         EF Core nos brinda diferentes formas de configurar las entidades, por "convención", "anotación de datos" y "apifluentes". Si hay algo que no puedo configurar por anotación de datos se debe 
         realizar con apifluentes.
        
         Anotación de datos: [StringLength(50)]

         Aplifluentes es un conjunto de métodos que podemos utilizar para realizar configuraciones de las entidades en EF Core, es la herramienta más poderosa que tiene EF.
         Para usar Apifluentes debo ir al ApplicationDbContext.cs y ver lo que escribi al respecto del método.


        */
        public int Id { get; set; }

        public string Nombre { get; set; } = null!; // null! significa perdonar el nulo
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();
    }
}
