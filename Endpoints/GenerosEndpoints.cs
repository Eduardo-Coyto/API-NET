using APINETEF.Entidades;
using APIPeli.DTOs;
using APIPeli.Filtros;
using APIPeli.Repositorios;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace APIPeli.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros (this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"))
                .RequireAuthorization();

            group.MapGet("/{id:int}", ObtenerGeneroPorId);

            /*
            Creamos un nuevo endpoint para manejar las solicitudes HTTP POST en la ruta "/generos". Este endpoint espera recibir dos parámetros: 
            - Genero genero: Representa los datos del género que se va a crear. Estos datos son proporcionados por el cuerpo de la solicitud HTTP.
            - IRepositorioGeneros repositorio: Es una dependencia que será resuelta por el contenedor de servicios. Este repositorio abstrae el acceso a datos para la entidad "Genero".

            IRepositorioGenero no es algo que va a pasar el usuario, lo vamos a sacar del contenedor generador de controles que nos permite tener la instancia que nos da este servicio.

            Dentro del bloque async, llamamos al método "Crear" del repositorio de géneros (IRepositorioGeneros) para almacenar el nuevo género en la base de datos. 
            Este método devuelve el ID asignado al nuevo género.

            Finalmente, retornamos un resultado HTTP 201 Created, indicando la creación exitosa del recurso. Incluimos la ubicación del nuevo recurso "/generos/{id}" y los datos del género creado.

            Este enfoque sigue el principio de inversión de dependencias (SOLID), ya que dependemos de la abstracción (IRepositorioGeneros) en lugar de un tipo concreto. Esto facilita cambios futuros en la implementación del repositorio sin afectar el resto de la aplicación.
            */
            group.MapPost("/", CrearGenero)
                .AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>()
                .RequireAuthorization("esadmin");

            group.MapPut("/{id:int}", ActualizarGenero)
                .AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>()
                .RequireAuthorization("esadmin");

            group.MapDelete("/{id:int}", BorrarGenero).RequireAuthorization("esadmin");

            return group;
        }
        /*
            * Se utiliza TypeResult para que en swagger salga de la forma que se espera.
        */
        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros respositorio,
            IMapper mapper)
        {
            var generos = await respositorio.ObtenerTodos();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio,
            int id,
            IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO = mapper.Map<GeneroDTO>(genero);  
            return TypedResults.Ok(generoDTO);
        }

        static async Task<Results<Created<GeneroDTO>, ValidationProblem>> CrearGenero(CrearGeneroDTO crearGeneroDTO, 
            IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            
            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound, ValidationProblem>> ActualizarGenero(int id, 
            CrearGeneroDTO crearGeneroDTO, 
            IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore, 
            IMapper mapper)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id;
            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent, NotFound>> BorrarGenero(int id, IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}
