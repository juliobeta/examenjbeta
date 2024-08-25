namespace examenjbeta.ViewModels
{
    public class RegisterViewModel
    {
        public int StaffId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
        public int TiendaId { get; set; }
        public string Password { get; set; }
    }

}
