using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Order;
using ShoppingApp.Models.Cart;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ShoppingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderRepository _orderRepository;
        public long _currentUserId;

        public OrderController(ILogger<OrderController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Loading all orders placed by user
        /// </summary>
        /// <returns> List of orders </returns>
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("")]
        public ActionResult GetOrders()
        {
            try
            {
                GetCurrentId();
                List<OrderDetailDTO> orders = new List<OrderDetailDTO>();
                var ordersList = _orderRepository.GetOrders(_currentUserId);
                return View("~/Views/Order/Orders.cshtml", ordersList);
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Get current logined User Id
        /// </summary>
        /// <returns> Current userId from Jwt token </returns>
        public void GetCurrentId()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(HttpContext.Session.GetString("token"));
                _currentUserId = long.Parse(securityToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            }
            catch (Exception)
            {
                _currentUserId = 0;
            }
        }
    }
}
