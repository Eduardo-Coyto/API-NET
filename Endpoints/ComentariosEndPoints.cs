using APIPeli.DTOs;
using APIPeli.Entidades;
using APIPeli.Filtros;
using APIPeli.Repositorios;
using APIPeli.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace APIPeli.Endpoints
{
    public static class ComentariosEndPoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c =>
                c.Expire(TimeSpan.FromSeconds(60))
                .Tag("comentarios-get")
                .SetVaryByRouteValue(new string[] { "peliculaId" }));
            /*
             SetVaryByRouteValue configurar variación por un valor de ruta. O sea, 
             está diciendo que va a variar chache según parametro peliculaId. El fin es que si consulto por el comentario 1
             y mientras el cache está activo consulta por el comentario 2 no le traiga el comentario 1 
            */
            group.MapGet("/{id:int}", ObtenerPorId);
            
            group.MapPost("/", CrearComentario)
                .AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>()
                .RequireAuthorization();
            
            group.MapPut("/{id:int}", ActualizarComentario)
                .AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>()
                .RequireAuthorization();

            group.MapDelete("/{id:int}", BorrarComentario)
                .RequireAuthorization();

            return group;
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId,
            IRepositorioComentarios repositorioComentarios, 
            IRepositorioPeliculas repositorioPeliculas,
            IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, 
            int id,
            IRepositorioComentarios repositorio, 
            IMapper mapper)
        {
            var comentario = await repositorio.ObtenerPorId(id);

            if (comentario is null)
            {
                return TypedResults.NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>, NotFound, BadRequest<string>>> CrearComentario(int peliculaId,
            CrearComentarioDTO crearComentarioDTO, 
            IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, 
            IMapper mapper, 
            IOutputCacheStore outputCacheStore,
            IServicioUsuarios servicioUsuarios)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;

            // primero busco el usuario que esta realizando el comentario.

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            comentario.UsuarioId = usuario.Id;

            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/comentario/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> ActualizarComentario(int peliculaId, 
            int id,
            CrearComentarioDTO crearComentarioDTO, 
            IOutputCacheStore outputCacheStore,
            IRepositorioComentarios repositorioComentarios, 
            IRepositorioPeliculas repositorioPeliculas,
            IServicioUsuarios servicioUsuarios)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            // luego de verificar que la pelicula existe quiero obtener el comentario por Id
            var comentarioBD = await repositorioComentarios.ObtenerPorId(id);

            if (comentarioBD is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioBD.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            comentarioBD.Cuerpo = crearComentarioDTO.Cuerpo;

            await repositorioComentarios.Actualizar(comentarioBD);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> BorrarComentario(int peliculaId, 
            int id,
            IRepositorioComentarios repositorio, 
            IOutputCacheStore outputCacheStore,
            IServicioUsuarios servicioUsuarios)
        {
            var comentarioBD = await repositorio.ObtenerPorId(id);

            if (comentarioBD is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioBD.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }


    }
}
