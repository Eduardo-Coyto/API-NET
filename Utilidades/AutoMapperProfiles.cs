using APINETEF.Entidades;
using APIPeli.DTOs;
using APIPeli.Entidades;
using AutoMapper;

namespace APIPeli.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();
            
            CreateMap<CrearActorDTO, Actor>()
                .ForMember(x => x.Foto, op => op.Ignore());
            CreateMap<Actor, ActorDTO>(); 
            
            CreateMap<CrearPeliculaDTO, Pelicula>()
                .ForMember(x => x.Poster, op => op.Ignore());

            /*
            Estoy indicando cómo vamos hacer el mapeo. Acá estoy mapeando de GeneroPelicula a GeneroDTO.
            Lo mismo hay que hacer con los Actores. Cuando pueda analizarlo mejor al mapeo
            Video 93
            */
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(p => p.Generos,
                entidad => entidad.MapFrom(p =>
                p.GenerosPeliculas.Select(gp =>
                    new GeneroDTO { Id = gp.GeneroId, Nombre = gp.Genero.Nombre })))
                .ForMember(p => p.Actores, 
                entidad => entidad.MapFrom(p =>
                p.ActoresPeliculas.Select(ap =>
                    new ActorPeliculaDTO
                    {
                        Id = ap.ActorId,
                        Nombre = ap.Actor.Nombre,
                        Personaje = ap.Personaje
                    })));

            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
           
            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();


        }
    }
}
