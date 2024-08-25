using examenjbeta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using examenjbeta.ViewModels;

namespace examenjbeta.Controllers
{
    public class OrdersController : Controller
    {
        // Instancia del contexto de base de datos
        private dbSistemaEntities1 db = new dbSistemaEntities1();

        // GET: Orders/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Orders/CreateOrder
        [HttpGet]
        public ActionResult CreateOrder()
        {
            // Inicializar el modelo con datos predeterminados
            var model = new OrderViewModel
            {
                OrderItems = new List<OrderItemViewModel>
                {
                    new OrderItemViewModel() // Inicializar con una fila vacía
                },
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now.AddDays(7)
            };

            // Cargar listas para los select dropdowns en la vista
            LoadDropDownLists();

            return View(model);
        }

        // POST: Orders/CreateOrder
        [HttpPost]
        public ActionResult CreateOrder(OrderViewModel model)
        {
            // Verifica si la acción se está llamando
            System.Diagnostics.Debug.WriteLine("CreateOrder action called");

            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Model is valid");
                System.Diagnostics.Debug.WriteLine("Customer ID: " + model.CustomerId);
                System.Diagnostics.Debug.WriteLine("Order Date: " + model.OrderDate);

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in model.OrderItems)
                        {
                            System.Diagnostics.Debug.WriteLine("Product ID: " + item.ProductId);
                            System.Diagnostics.Debug.WriteLine("Quantity: " + item.Quantity);

                            var productStock = db.Stocks.FirstOrDefault(s => s.product_id == item.ProductId && s.store_id == model.StoreId);
                            if (productStock == null || productStock.quantity < item.Quantity)
                            {
                                ModelState.AddModelError("", $"No hay stock suficiente para el producto {item.ProductId}");
                                LoadDropDownLists();
                                return View(model);
                            }
                        }

                        // Crear la orden de compra
                        var order = new Orders
                        {
                            customer_id = model.CustomerId,
                            order_date = model.OrderDate,
                            required_date = model.RequiredDate,
                            store_id = model.StoreId,
                            staff_id = model.UserId,
                            order_status = 1
                         };
                        db.Orders.Add(order);
                        db.SaveChanges();

                        // Agregar los elementos de la orden
                        foreach (var item in model.OrderItems)
                        {
                            db.Order_items.Add(new Order_items
                            {
                                order_id = order.order_id,
                                product_id = item.ProductId,
                                quantity = item.Quantity,
                                list_price = item.ListPrice,
                                discount = item.Discount
                            });

                            var productStock = db.Stocks.FirstOrDefault(s => s.product_id == item.ProductId && s.store_id == model.StoreId);
                            if (productStock != null)
                            {
                                productStock.quantity -= item.Quantity;
                            }
                        }
                        db.SaveChanges();

                        // Confirmar la transacción
                        transaction.Commit();

                        // Redirigir a una vista de detalles de la orden
                        return RedirectToAction("OrderDetails", new { id = order.order_id });
                    }
                    catch (Exception ex)
                    {
                        // Revertir la transacción si algo falla
                        transaction.Rollback();
                        ModelState.AddModelError("", "Ocurrió un error al crear la orden. Por favor, intente nuevamente.");
                        LoadDropDownLists();
                        return View(model);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Model is not valid");
            }

            // Si el modelo no es válido, recargar los dropdowns y volver a mostrar la vista
            LoadDropDownLists();
            return View(model);
        }

        // GET: Orders/OrderDetails/{id}
        public ActionResult OrderDetails(int id)
        {
            var order = db.Orders.Include("Order_items").FirstOrDefault(o => o.order_id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // Acción para obtener información del producto
        public JsonResult GetProductInfo(int productId)
        {
            var product = db.Products
                            .Where(p => p.product_id == productId)
                            .Select(p => new
                            {
                                ProductName = p.product_name,
                                ListPrice = p.list_price
                            })
                            .FirstOrDefault();

            if (product == null)
            {
                return Json(new { error = "Producto no encontrado" }, JsonRequestBehavior.AllowGet);
            }

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        // Método auxiliar para cargar los datos de los dropdowns
        private void LoadDropDownLists()
        {
            ViewBag.Customers = new SelectList(db.Customers.Select(c => new
            {
                customer_id = c.customer_id,
                full_name = c.first_name + " " + c.last_name
            }).ToList(), "customer_id", "full_name");

            ViewBag.Stores = new SelectList(db.Stores, "store_id", "store_name");
            ViewBag.Users = new SelectList(db.Staffs.Select(s => new
            {
                staff_id = s.staff_id,
                full_name = s.first_name + " " + s.last_name
            }).ToList(), "staff_id", "full_name");
        }
    }
}
