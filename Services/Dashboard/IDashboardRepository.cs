using Domain.Model;
using Domain.Model.Dashboard;
using Domain.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Dashboard
{
    public interface IDashboardRepository
    {
        IEnumerable<Productlist> GetAllProduct(long currentuserid);
        IEnumerable<Productlist> GetProductLowHigh(long currentuserid);
        IEnumerable<Productlist> GetProductHighLow(long currentuserid);
        IEnumerable<Productlist> GetSearchItems(string searchString);
        void InsertProduct(Productlist entity);
        Productlist GetProduct(long id);
        void DeleteProduct(long id);
        void AddtoCart(long productid, long userid);
        string CheckingRole(long userid);
    }

}
