using Domain.Model.User;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using ShoppingApp.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Login
{
    public interface ILoginRepository
    {
        User GetUserByEmail(string email, string password);

        ForgetPassword ForgetPassword(ForgetPasswordModel forgetPasswordModel);
    }
}
