using Domain.Model.User;
using Microsoft.AspNetCore.Mvc;
using Services.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace ShoppingApp.Controllers
{
    public class UserController : Controller
    {
        IUserRepository _iuserRepository;
        public long _currentUserId;

        public UserController(IUserRepository iuserRepository)
        {
            this._iuserRepository = iuserRepository;
        }

        /// <summary>
        /// Listing all users registerd for admin role - not in use
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListUsers()
        {
            try
            {
                List<User> lstUser = new List<User>();
                _iuserRepository.GetUsers().ToList().ForEach(u =>
                {
                    User user = null;
                    user = new User()
                    {
                        Id = u.Id,
                        firstName = u.firstName,
                        lastName = u.lastName,
                        email = u.email,
                        phoneNumber = u.phoneNumber,
                        password = u.password,
                        policyFlag = u.policyFlag,
                        Role = u.Role,
                    };
                    lstUser.Add(user);
                });

                ViewData["lstUser"] = lstUser;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Loading the empty Register view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Registration(int id = 0)
        {
            try
            {
                User user = new User();
                string[] roles = { Role.Admin, Role.Customer };
                ViewData["roles"] = roles;
                return View(user);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Loading user data view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetUser")]
        public ActionResult GetUser()
        {
            try
            {
                GetCurrentId();
                User user = _iuserRepository.GetUser(_currentUserId);
                string[] roles = { Role.Admin, Role.Customer };
                ViewData["roles"] = roles;
                return View("GetUser", user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// After registration showing success message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RegistrationSuccess()
        {
            return View("UserRegistrationSuccess");
        }

        /// <summary>
        /// After update showing success message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UpdateSuccess()
        {
            return View("UserUpdateSuccess");
        }

        /// <summary>
        /// Save the user info into table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(User user)
        {
            try
            {
                if (user.email != "" && user.firstName != "" && user.email != null && user.firstName != null && user.password != "" && user.password != null && user.password == user.ConfirmPassword)
                {
                    User loginUser = new User
                    {
                        firstName = user.firstName,
                        ConfirmPassword = user.ConfirmPassword,
                        email = user.email,
                        lastName = user.lastName,
                        password = Models.EncDec.Encrypt(user.password),
                        Role = user.Role,
                        phoneNumber = user.phoneNumber,
                        policyFlag = user.policyFlag
                    };

                    var inserted = _iuserRepository.InsertUser1(loginUser);
                    if (inserted == 1)
                    {
                        return Json(new { success = true, message = "Success" });
                    }
                    else
                    {
                        return Json(new { success = true, message = "Email already exists" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Fill all mandatory fields" });
                }
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// Updating the user information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateUser(User user)
        {
            try
            {
                if (user.email != "" && user.firstName != "" && user.email != null && user.firstName != null && user.password != "" && user.password != null)
                {
                    User loginUser = new User
                    {
                        Id=user.Id,
                        firstName = user.firstName,
                        ConfirmPassword = user.ConfirmPassword,
                        email = user.email,
                        lastName = user.lastName,
                        password = Models.EncDec.Encrypt(user.password),
                        Role = user.Role,
                        phoneNumber = user.phoneNumber,
                        policyFlag = user.policyFlag
                    };

                    _iuserRepository.UpdatetUser(loginUser);
                    var inserted = 1;
                    if (inserted == 1)
                    {
                        return Json(new { success = true, message = "Success" });
                    }
                    else
                    {
                        return Json(new { success = true, message = "Email already exists" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Fill all mandatory fields" });
                }
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, exception.Message });
            }
        }

        /// <summary>
        /// Delete a particular user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteUser")]
        public int DeleteUser(long id)
        {
            try
            {
                _iuserRepository.DeleteUser(id);
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check email already exists from the new registration window
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmailExists(User model)
        {
            try
            {
                if (_iuserRepository.EmailExists(model.email))
                {
                    return Json(new { success = true, message = "Email already exists" });
                }
                else
                {
                    return Json(new { success = false, message = "Email Availabe!" });
                }
            }
            catch (Exception)
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