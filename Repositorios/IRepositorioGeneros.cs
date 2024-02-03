using APINETEF.Entidades;

namespace APIPeli.Repositorios
{
    /*
     
     "IRepositorioGeneros". Las interfaces proporcionan una forma de definir métodos que las clases deben implementar.
      Task<int> Crear(Genero genero): Declara un método llamado Crear que toma un parámetro de tipo Genero llamado genero y devuelve un Task<int>.

      Task<int>: Representa una operación asincrónica que devuelve un resultado de tipo int. La asincronía es útil para operaciones de E/S, como acceder a bases de datos, 
      ya que no bloquea el hilo principal de la aplicación mientras espera que se complete la operación.
    
      "Genero?": Indica que puede tener un resultado nulo.
     
     
    */

    public interface IRepositorioGeneros
    {
        Task<List<Genero>> ObtenerTodos();
        Task<Genero?> ObtenerPorId(int id);
        Task<int> Crear(Genero genero);
        Task<bool> Existe (int id);
        Task<List<int>> Existen(List<int> ids);
        Task Actualizar(Genero genero);
        Task Borrar(int id);
        Task<bool> Existe(int id, string nombre);
    }
}
