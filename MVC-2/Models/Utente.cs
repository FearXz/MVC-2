namespace MVC_2.Models
{
    public class Utente
    {
        public int IdUtente { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool TipoUtente { get; set; }
    }
}