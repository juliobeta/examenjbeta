namespace examenjbeta.ViewModels
{
    // Esta clase sirve como ViewModel para la vista de Login.
    // Un ViewModel es un objeto que contiene la información
    // necesaria para una vista específica en la aplicación.
    public class LoginViewModel
    {
        // Propiedad para almacenar el nombre de usuario que se ingresará en la vista de login.
        // Esta propiedad será vinculada a un campo de entrada en la vista.
        public string Username { get; set; }

        // Propiedad para almacenar la contraseña que se ingresará en la vista de login.
        // Al igual que 'Username', esta propiedad será vinculada a un campo de entrada en la vista.
        public string Password { get; set; }
    }
}
