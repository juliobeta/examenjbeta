namespace examenjbeta.ViewModels
{
    // ViewModel utilizado para capturar los datos del formulario de registro de usuario
    public class RegisterViewModel
    {
        // Identificación del Staff. Este valor se vincula con el campo staff_id en la base de datos
        public int StaffId { get; set; }

        // Nombre del usuario que se está registrando
        public string Nombre { get; set; }

        // Apellido del usuario que se está registrando
        public string Apellido { get; set; }

        // Dirección de correo electrónico del usuario. Utilizada también para generar el nombre de usuario
        public string Correo { get; set; }

        // Indica si el usuario está activo o no. Se convierte a un byte (1 o 0) para almacenarlo en la base de datos
        public bool Activo { get; set; }

        // Identificación de la tienda seleccionada por el usuario durante el registro
        public int TiendaId { get; set; }

        // Contraseña del usuario que se encripta antes de almacenarse en la base de datos
        public string Password { get; set; }
    }
}

