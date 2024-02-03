
using APIPeli.Entidades;

namespace APIPeli.Repositorios
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}