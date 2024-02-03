using Microsoft.EntityFrameworkCore;
using APIPeli.DTOs;
using APIPeli.Entidades;
using APIPeli.Servicios;
using APIPeli.Utilidades;
using AutoMapper;

namespace APIPeli.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }
        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await httpContext.InsertarParametrosPAginacionEnCabecera(queryable);
            return await queryable.OrderBy(p => p.Titulo)
                .Paginar(paginacionDTO).ToListAsync();
        }
        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas
                .Include(p => p.Comentarios)
                .Include(p => p.GenerosPeliculas)
                    .ThenInclude(gp => gp.Genero) 
                    /*
                    Estoy yendo hasta mi tabla de Peliculas, le pido que me incluya los Generos de la pelicula con 
                    Include en GenerosPeliculas y además quiero icluir de la tabla GenerosPelicula los Generos. 
                    Por lo tanto con ThenInclude se puede lograr esa acción.
                    */
                .Include(p => p.ActoresPeliculas.OrderBy(a => a.Orden))
                    .ThenInclude(ap => ap.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }
        public async Task Actualizar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(p => p.Id == id);
        }

        public async Task AsignarGeneros(int id, 
            List<int> generosIds)
        {
            var pelicula = await context.Peliculas
                .Include(p => p.GenerosPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe una película con el id {id}");
            }

            var generosPeliculas = generosIds.Select(generoId => new GeneroPelicula() { GeneroId = generoId }); //esto es una proyección. Si estoy trayendo 3 GenerosId estoy generando un IEnumerable de GenerosPeliculas de 3 elementos, uno por cada GeneroId que reciba

            pelicula.GenerosPeliculas = mapper.Map(generosPeliculas, pelicula.GenerosPeliculas);
            /*
            aca estamos mapeando generoPeliculas hacia pelicula.GeneroPeliculas lo cual va a editar el contenido de GenerosPeliculas manteniendo la misma instancia que ya se tenia en GenerosPeliculas
            en EF Core si yo edito en memoria mis entidades esos cambios se pueden reflejar en la BD haciendo SaveChanges.
            AutoMapper me permite editar el listado de GeneroPeliculas a partir del listado generosPeliculas
            Utilizando este código pelicula.GenerosPeliculas = mapper.Map(generosPeliculas, pelicula.GenerosPeliculas); estamos manteniendo la misma instancia de GenerosPeliculas lo cual me permite editar en memoria dicho listado, por lo tanto cuando haga SaveChagesAsync eso va mandar los cambios la BD con los nuevos generos
            */
            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int id, 
            List<ActorPelicula> actores)
        {
            for (int i = 1; i <= actores.Count; i++)
            {
                actores[i - 1].Orden = i;
            }

            var pelicula = await context.Peliculas
                .Include(p => p.ActoresPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe la película con id: {id}");
            }

            pelicula.ActoresPeliculas = mapper.Map(actores, pelicula.ActoresPeliculas);

            await context.SaveChangesAsync();
        }

    }
}
