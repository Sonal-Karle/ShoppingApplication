using Domain.Model.Cart;
using Domain.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Cart;
using ShoppingApp.Models.Cart;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ShoppingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartProductRepository _cartProductRepository;
        public long _currentUserId;

        public CartController(ILogger<CartController> logger, ICartProductRepository cartProductRepository)
        {
            _logger = logger;
            _cartProductRepository = cartProductRepository;
        }

        public IActionResult Index()
        {
            try
            {
                GetCurrentId();
                List<CartProducts> cartProducts = new List<CartProducts>();
                IEnumerable<CartProducts> cartItems = cartProducts;
                cartItems = _cartProductRepository.GetCartProducts(_currentUserId);                            
                return View("CartItems", cartItems);
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }
        /// <summary>
        /// Loading all the user products in cart
        /// </summary>
        /// <returns>List of products in cart</returns>
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("")]
        public IActionResult CartItems()
        {
            try
            {
                GetCurrentId();
                List<CartProducts> cartProducts = new List<CartProducts>();
                IEnumerable<CartProducts> cartItems = cartProducts;
                cartItems = _cartProductRepository.GetCartProducts(_currentUserId);                            
                ViewBag.addresses = _cartProductRepository.LoadUserAddress(_currentUserId);
                return View(cartItems);
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// Get the number of items in cart
        /// </summary>
        /// <returns> Int count of items </returns>        
        [HttpGet("GetCartCount")]
        public IActionResult GetCartCount()
        {
            try
            {
                GetCurrentId();
                int countProduct=0;
                countProduct = _cartProductRepository.GetCartProducts(_currentUserId).Count();
                return Json(new { success = true, message = countProduct });
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }
        /// <summary>
        /// Updating the item count in user cart
        /// </summary>
        /// <param name="cartProducts"></param>
        /// <returns>Suceess true and false</returns>
        [HttpPost("UpdateCart")]
        public IActionResult UpdateCart(List<CartProductsDTO> cartProducts)
        {
            try
            {
                GetCurrentId();
                foreach (var cartItem in cartProducts)
                {
                    _cartProductRepository.UpdateProduct(cartItem.productId, _currentUserId, cartItem.count);
                }
                return Json(new { success = true, message = "Success" });
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// Placing the order and moving the cart data to OrderHeader and OrderDetails
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns>Suceess true and false</returns>
        [HttpPost("Checkout")]
        public IActionResult Checkout(OrderDetailDTO orderDetail)
        {
            try
            {
                GetCurrentId();
                _cartProductRepository.Checkout(_currentUserId, orderDetail.addressId);
                _cartProductRepository.EmptyCart(_currentUserId);
                return Json(new { success = true, message = "Success" });
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// Remove a single product from cart
        /// </summary>
        /// <param name="cartProducts"></param>
        /// <returns>Suceess true and false</returns>
        [HttpPost("RemoveProductFromCart")]
        public IActionResult RemoveProductFromCart(CartProductsDTO cartProducts)
        {
            try
            {
                GetCurrentId();
                _cartProductRepository.DeleteProduct(cartProducts.productId, _currentUserId);
                return Json(new { success = true, message = "Success" });
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// User have multiple addresses loading that all in to dropdown
        /// </summary>
        /// <returns> List of address </returns>
        [HttpGet("LoadUserAddress")]
        public IActionResult LoadUserAddress()
        {
            try
            {
                GetCurrentId();
                List<Address> address = _cartProductRepository.LoadUserAddress(_currentUserId);
                return Json(new { address, message = "Success" });
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// From the modal-address saving data into Address table
        /// </summary>
        /// <param name="address"></param>
        /// <returns> Suceess true and false </returns>
        [HttpPost("SaveUserAddress")]
        public IActionResult SaveAddress(Address address)
        {
            try
            {
                GetCurrentId();
                _cartProductRepository.SaveUserAddress(address, _currentUserId);
                return Json(new { success = true, message = "Success" });
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// After placing order showing success message and redirect to past orders
        /// </summary>
        /// <returns></returns>
        [HttpGet("CheckoutSuccess")]
        public IActionResult CheckoutSuccess()
        {
            GetCurrentId();
            ViewBag.userName = _cartProductRepository.GetUserName(_currentUserId);
            return View("CheckoutSuccess");
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
