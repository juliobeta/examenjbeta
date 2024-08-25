using System.Collections.Generic;
using System;

namespace examenjbeta.ViewModels
{
    public class OrderViewModel
    {
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public int StoreId { get; set; }
        public int UserId { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
