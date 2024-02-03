namespace APIPeli.DTOs
{
    public class RespuestaAutenticacionDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expiracion { get; set; }
    }
}
