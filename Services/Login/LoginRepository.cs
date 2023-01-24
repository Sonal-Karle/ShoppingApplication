using Domain.EntityFramework;
using Domain.Model.User;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Login.Login;
using Services.Registration;
using ShoppingApp.Models.Login;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IUserRepository userRepository;
        ApplicationDbContext db = new ApplicationDbContext();
        private IConfiguration Configuration { get; }


        public LoginRepository(IUserRepository userRepository,IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.Configuration = configuration;
        }
        

        public User GetUserByEmail(string email, string password)
        {
            //Retriving all user by calling method from userrepository
            //Using linq match the email and password combination present in table or not
            var allUsers = userRepository.GetUsers();
            var user = allUsers.SingleOrDefault(x => (x.email == email && x.password == password));
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public ForgetPassword ForgetPassword(ForgetPasswordModel forgetPasswordModel)
        {
            ForgetPassword item = new ForgetPassword();
            ForgetModel forget = new ForgetModel();
            User login = new User();
            login.email = forgetPasswordModel.email;
            User validateEmail = db.Register
                      .Where(e => e.email.Equals(login.email)).FirstOrDefault(e => e.email == login.email );
            var jwt = GenerateToken(validateEmail);
            forget.JwtToken = jwt;


            if (validateEmail != null)
            {
                var model1 = new ForgetPasswordModel { email = forgetPasswordModel.email };
                var model2 = new ForgetModel { JwtToken = forget.JwtToken };
                var model = new ForgetPassword { Email = model1.email, JwtToken = model2.JwtToken };
                return model;
            }
            else
            {
                return item;
            }


        }

        public string GenerateToken(User login)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {

                  new Claim("Id", login.Id.ToString()),
                  new Claim("email", login.email),

              };
            var token = new JwtSecurityToken(Configuration["Jwt:Issuer"],
               Configuration["Jwt:Issuer"],
               claims,
               expires: DateTime.Now.AddMinutes(120),
               signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
