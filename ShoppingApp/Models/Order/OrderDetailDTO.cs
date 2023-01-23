using Domain;
using Domain.Model.Dashboard;
using Domain.Model.User;
using System;
using System.Collections.Generic;

namespace ShoppingApp.Models.Cart
{
    public class OrderDetailDTO 
    {
        public long userId { get; set; }
        public long addressId { get; set; }
        public int productId { get; set; }

        public int orderNumber { get; set; }
        public Address Address { get; set; }
        public Domain.Model.User.User User { get; set; }
        public DateTime orderDate { get; set; }
        public List<Productlist> ProductList { get; set; }
        public int count { get; set; }
        public long orderHeaderId { get; set; }

    }
}
