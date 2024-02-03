using APINETEF.Entidades;
using Microsoft.EntityFrameworkCore;

namespace APIPeli.Repositorios
{
    public class RepositorioGeneros : IRepositorioGeneros


        /*
        
        Para poder crear una entidad en EF Core necesitamos una instancia de DbContext.
        
        Constructor de la clase RepositorioGeneros que recibe una instancia de ApplicationDbContext.
        Esto sigue el principio de inyección de dependencias, ya que el contexto de la base de datos es proporcionado externamente.
       
        Método asincrónico que implementa la operación de creación de un nuevo género en la base de datos.
        
        */

        
    {
        private readonly ApplicationDbContext context;

        public RepositorioGeneros(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<int> Crear(Genero genero)
        {
            context.Add(genero);
            await context.SaveChangesAsync();
            return genero.Id;
        }
        public async Task Actualizar(Genero genero)
        {
            context.Update(genero);
            await context.SaveChangesAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Generos.AnyAsync(x => x.Id == id);
        }

        //se realiza este método para verificar con validaciones
        public async Task<bool> Existe(int id, string nombre)
        {
            return await context.Generos.AnyAsync(g => g.Id != id && g.Nombre == nombre);// si es distinto id pero mismo nombre es un problema
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Generos.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync();
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {
            return await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Genero>> ObtenerTodos()
        {
            return await context.Generos.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }
}
