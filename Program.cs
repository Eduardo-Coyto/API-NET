

using APIPeli;
using APIPeli.Endpoints;
using APIPeli.Migrations;
using APIPeli.Repositorios;
using APIPeli.Servicios;
using APIPeli.Utilidades;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!; // con la expresi�n "!" me aseguro que no ser� nulo

// Inicio del �rea de los SERVICIOS. Es una manera de configurar clases que quieras reutilizar a lo largo de la app

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

// podemos usar Identitu con nuestra instancia del ApplicactionDbContext
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// servicio para crear usuarios
builder.Services.AddScoped<UserManager<IdentityUser>>();
// servicio para logear usarios.
builder.Services.AddScoped<SignInManager<IdentityUser>>();

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });  
    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });

});

builder.Services.AddOutputCache();

// configuro el servicio de swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* 
 * configuro el servicio para utilziar IRepositorioGeneros en cualquier parte de la app
 * con esto dependo de una abstracci�n y no un tipo concreto. Asi implemento el 5to principio SOLID "principio de inversi�n de dependencias". Debemos depender de abstracciones y no tipos concretos.
 * Si ma�ana quiero cambiar RepositoriosGeneros lo hago de un solo lugar
*/
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivoLocal>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

// agrego ete servicio para tener disponibilidad del servicio creado IHttpContextAccesor 
builder.Services.AddHttpContextAccessor();

//configuramos AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// configuramos AddProblemDetails el cual me permite manjerar las configuraciones con respecto al manejo de errores o problemas
builder.Services.AddProblemDetails();


//configuramos los servicios de autorizaci�n y autenticaci�n.

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(),
        IssuerSigningKeys = Llaves.ObtenerTodasLasLlaves(builder.Configuration),
        ClockSkew = TimeSpan.Zero
    };

});

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});


// Fin del �rea de los SERVICIOS

var app = builder.Build();

// Inicio del �rea de los MIDDLEWARE. Define cada una de las acciones que queremos ejecutar cada vez que una acci�n http sea recibida por nuestra app

app.UseSwagger();
app.UseSwaggerUI(); //configuro para usar la ui de swagger 

//utilizo manejo de excepciones
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();
    var excepcion = exceptionHandleFeature?.Error!;

    var error = new APIPeli.Entidades.Error();
    error.Fecha = DateTime.UtcNow;
    error.MensajeDeError = excepcion.Message;
    error.StackTrace = excepcion.StackTrace;

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>();
    await repositorio.Crear(error);

    await TypedResults.BadRequest(
        new { tipo = "error", mensaje = "ha ocurrido un mensaje de error inesperado", estatus = 500 })
    .ExecuteAsync(context);
}));
app.UseStatusCodePages(); //configurar nuestra aplicaci�n para obtner el codigo de status cuando hay un error

// tengo que permitir que los clientes puedan acceder a trav�s de este middleware a los archivos estaticos en wwwroot
app.UseStaticFiles();
app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("error de ejemplo");
});



// Ac� tengo configurado todos los endpoints del grupo de Generos.
app.MapGroup("/generos").MapGeneros();
// Ac� tengo configurado todos los endpoints del grupo de Actores.
app.MapGroup("/actores").MapActores();
// Ac� tengo configurado todos los endpoints del grupo de Peliculas.
app.MapGroup("/peliculas").MapPeliculas();
// Ac� tengo configurado todos los endpoints del grupo de Comentarios.
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();
// Ac� tengo configurado todos los endpoints del grupo de Comentarios.
app.MapGroup("/usuarios").MapUsuarios();


// Fin del �rea de los MIDDLEWARE


app.Run();

