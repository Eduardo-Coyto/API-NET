using APINETEF.Entidades;
using APIPeli.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIPeli
{
    /*
     
    Aqui se realiza la configuración central de EF Core 

    Como estamos utilizando Bases de Datos primero utilizamos utilizamos una clase que hereda del DbContext. 
    DbContext el la pieza central de EF Core, a través de esta clase podemos configurar nuestra Base de Datos y consultas a las distintas tablas entre otras cosas.

    DbSet me permite configurar las tablas de mi base de datos y la estructura de la misma sera en este caso lo que tenga la estructura <Genero>. Al hacer esta configuración, voy a 
    crear una tabla en la base de datos con el nombre Generos con la columna Id y Nombre

    Necesitamos crear una migración el cual nos permite ver qué es lo que está pasando con la base de datos. Este es un paso intermedio antes de trabajar con la BD. Nos permite verificar qué es lo que estamos cambiando o realizando de la misma.
    Para crear una migración vamos a utilizar el packmanagmet console (Herramientas -> Administrador de paquetes de Nuguet -> Consola de paquete)
    y debo escribir el comando "Add-Migration Generos" Generos es el nombre de la migración el cual se puede utilizar una unica vez.

    Para crear o actualizar la BD debo colocar en la consola packmanagmet (Herramientas -> Administrador de paquetes de Nuguet -> Consola de paquete), agrego el comando "Update-Database"

    Para usar apifluentes que debo usar el método override OnModelCreatin.

    En el video 120 cambio la forma de Heredar. Antes estaba en el DbContext (tener en cuenta para agregar a la bitacora)

    */
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genero>().Property(p=>p.Nombre).HasMaxLength(50);

            modelBuilder.Entity<Actor>().Property(p=>p.Nombre).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(p=>p.Foto).IsUnicode();
            
            modelBuilder.Entity<Pelicula>().Property(p=>p.Titulo).HasMaxLength(150);
            modelBuilder.Entity<Pelicula>().Property(p=>p.Poster).IsUnicode();

            modelBuilder.Entity<GeneroPelicula>().HasKey(g => new { g.GeneroId, g.PeliculaId }); // la llave primaria de la tabla GeneroPelicula sera una llave compuesta entre GeneroId y PeliculaId
            
            modelBuilder.Entity<ActorPelicula>().HasKey(g => new { g.ActorId, g.PeliculaId }); // la llave primaria de la tabla ActorPelicula sera una llave compuesta entre ActorId y PeliculaId

            modelBuilder.Entity<IdentityUser>().ToTable("Usuarios");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsuariosClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsuariosLogins");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsuariosRoles");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsuariosTokens");
        }

        public DbSet<Genero> Generos { get; set; } // voy a utilizar la entidad de Genero para crear la tabla Generos en la base de datos
        public DbSet<Actor> Actores { get; set; } // voy a utilizar la entidad de Actor para crear la tabla Actores en la base de datos
        public DbSet<Pelicula> Peliculas { get; set; } // voy a utilizar la entidad de Pelicula para crear la tabla Peliculas en la base de datos
        public DbSet<Comentario> Comentarios { get; set; } // voy a utilizar la entidad de Comentario para crear la tabla Comentarios en la base de datos
        public DbSet<GeneroPelicula> GenerosPeliculas { get; set; } // voy a utilizar la entidad de GeneroPelicula como tabla intermedia para relacion muchos a muchos entre Genero y Pelicula para crear la tabla GenerosPeliculas en la base de datos
        public DbSet<ActorPelicula> ActoresPeliculas { get; set; } // voy a utilizar la entidad de ActorPelicula como tabla intermedia para relacion muchos a muchos entre Actor y Pelicula para crear la tabla ActoresPeliculas en la base de datos
        public DbSet<Error> Errores { get; set; } // voy a utilizar la entidad de Error para crear la tabla Errores en la base de datos
    }
}
