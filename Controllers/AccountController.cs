using examenjbeta.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using examenjbeta.ViewModels;
using System.Data.SqlClient;

namespace examenjbeta.Controllers
{
    public class AccountController : Controller
    {
        // Contexto de la base de datos
        private dbSistemaEntities1 db = new dbSistemaEntities1();

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Muestra la vista de login
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Busca un usuario en la base de datos con las credenciales proporcionadas
                var user = db.SystemUser.FirstOrDefault(u => u.usuario == model.Username && u.pass == model.Password && u.Staffs.active == 1);
                if (user != null)
                {
                    // Si el usuario es válido, redirige a la página de creación de órdenes
                    return RedirectToAction("CreateOrder", "Orders");
                }
                // Si las credenciales son incorrectas, muestra un mensaje de error
                ModelState.AddModelError("", "Usuario y/o contraseña incorrecta");
            }
            // Si el modelo no es válido o las credenciales son incorrectas, vuelve a mostrar la vista de login con los errores
            return View(model);
        }

        // GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            // Carga las tiendas en ViewBag para mostrarlas en un dropdown en la vista de registro
            ViewBag.Tiendas = db.Stores.ToList();
            // Muestra la vista de registro
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            // Muestra un mensaje en la consola para depuración
            System.Diagnostics.Debug.WriteLine("CreateOrder action called");

            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Busca un usuario existente basado en el correo electrónico
                var existingUser = db.Staffs.FirstOrDefault(s => s.email == model.Correo);
                if (existingUser != null)
                {
                    // Si el usuario ya existe, actualiza su información
                    existingUser.first_name = model.Nombre;
                    existingUser.last_name = model.Apellido;
                    existingUser.email = model.Correo;
                    existingUser.store_id = model.TiendaId;
                    existingUser.active = model.Activo ? (byte)1 : (byte)0;
                    db.SaveChanges();
                }
                else
                {
                    // Si el usuario no existe, crea un nuevo registro en la tabla Staffs
                    var newStaff = new Staffs
                    {
                        first_name = model.Nombre,
                        last_name = model.Apellido,
                        email = model.Correo,
                        store_id = model.TiendaId,
                        active = model.Activo ? (byte)1 : (byte)0,
                        staff_id = model.StaffId
                    };

                    // Agrega el nuevo Staff a la base de datos y guarda los cambios
                    db.Staffs.Add(newStaff);
                    db.SaveChanges();

                    // Llama al procedimiento almacenado CreateUser para insertar en SystemUser
                    string correo = model.Correo;
                    string clave = model.Password;

                    try
                    {
                        // Ejecuta el procedimiento almacenado para insertar el nuevo usuario en SystemUser
                        db.Database.ExecuteSqlCommand(
                            "EXEC CreateUser @correo, @clave, @staff_id",
                            new SqlParameter("@correo", correo),
                            new SqlParameter("@clave", clave),
                            new SqlParameter("@staff_id", newStaff.staff_id) // staff_id es int y se pasa correctamente
                        );
                    }
                    catch (Exception ex)
                    {
                        // Maneja cualquier error que ocurra al ejecutar el procedimiento almacenado
                        throw new Exception("Error al ejecutar el procedimiento almacenado CreateUser", ex);
                    }
                }

                // Muestra un mensaje de éxito y redirige a la vista de registro para un nuevo ingreso
                TempData["Message"] = "Usuario registrado con éxito.";
                return RedirectToAction("Register");
            }

            // Si el modelo no es válido, recarga las tiendas y vuelve a mostrar la vista de registro con los errores
            ViewBag.Tiendas = db.Stores.ToList();
            return View(model);
        }

    }
}
