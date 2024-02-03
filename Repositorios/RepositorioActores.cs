using APIPeli.DTOs;
using APIPeli.Entidades;
using APIPeli.Servicios;
using APIPeli.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace APIPeli.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioActores(ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await httpContext.InsertarParametrosPAginacionEnCabecera(queryable);
            return await queryable.OrderBy(p => p.Nombre).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await context.Actores.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Actor>> ObtenerPorNombre(string nombre)
        {
            return await context.Actores.Where(p=>p.Nombre.Contains(nombre)).OrderBy(a=> a.Nombre).ToListAsync();
        }

        public async Task<int> Crear(Actor actor)
        {
            context.Actores.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Actores.AnyAsync(p => p.Id == id);
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Actores.Where(a => ids.Contains(a.Id)).Select(a => a.Id).ToListAsync();
        }

        public async Task Actualizar(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }
        public async Task Borrar(int id)
        {
            await context.Actores.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
    }
}
