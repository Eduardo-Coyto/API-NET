namespace APIPeli.Entidades
{
    public class Error
    {
        public Guid Id { get; set; } //Guid representa un string aleatorio
        public string MensajeDeError { get; set; } = null!;
        public string? StackTrace { get; set; }
        public DateTime Fecha { get; set; }
    }
}
