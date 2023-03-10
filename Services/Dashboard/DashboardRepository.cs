using Domain.EntityFramework;
using Domain.Model;
using Domain.Model.Cart;
using Domain.Model.Dashboard;
using Domain.Model.User;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        IGenericRepository<Productlist> _genericRepository;
        ApplicationDbContext db = new ApplicationDbContext();

        public DashboardRepository(IGenericRepository<Productlist> shoppingRepository)
        {
            this._genericRepository = shoppingRepository;
        }

        /// <summary>
        /// Get all the product from the table by using genericrepository 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Productlist> GetAllProduct(long currentuserid)
        {
            ResettingAddtoCart(currentuserid);
            List<Productlist> productlist = new List<Productlist>();
            _genericRepository.GetAll().ToList().ForEach(u =>
            {
                Productlist product = null;
                product = new Productlist()
                {
                    Id = u.Id,
                    ProductDescription = u.ProductDescription,
                    ProductPrice = u.ProductPrice,
                    ProductName = u.ProductName,
                    ProductImage = u.ProductImage,
                    InStock = u.InStock,
                    InCart = u.InCart,
                    Quantity = u.Quantity,
                };
                productlist.Add(product);
            });
            IEnumerable<Productlist> products = productlist;

            return products;

        }

        /// <summary>
        /// Get the product by Low to high by using linq statement 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Productlist> GetProductLowHigh(long currentuserid)
        {

            IEnumerable<Productlist> productlist = GetAllProduct(currentuserid);
            IEnumerable<Productlist> productsLH = productlist.OrderBy(x => x.ProductPrice);
            return productsLH;
        }

        /// <summary>
        /// Get the product by high to low by using linq statement 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Productlist> GetProductHighLow(long currentuserid)
        {

            IEnumerable<Productlist> productlist = GetAllProduct( currentuserid);
            IEnumerable<Productlist> productsLH = productlist.OrderByDescending(x => x.ProductPrice);
            return productsLH;
        }
        /// <summary>
        /// Insert the product in the table 
        /// </summary>
        /// <param name="model"></param>
        public void InsertProduct(Productlist model)
        {

            Productlist entity = null;
            entity = new Productlist
            {
                ProductDescription = model.ProductDescription,
                ProductPrice = model.ProductPrice,
                ProductName = model.ProductName,
                ProductImage = model.ProductImage,
                InStock = model.InStock,
                InCart = model.InCart,
                Quantity = model.Quantity,

            };
            _genericRepository.Insert(entity);
        }

        /// <summary>
        /// Get Particular product by passing ID fro that calling generic repository 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Productlist GetProduct(long id)
        {
            return _genericRepository.GetT(id);
        }

       

        /// <summary>
        /// Add the item in cart by passing ID 
        /// </summary>
        /// <param name="id"></param>
        public void AddtoCart(long productId, long userId)
        {
            var query = db.Register.Where(c => c.Id == userId).FirstOrDefault();
            var productquery = db.Productlist.Where(c => c.Id == productId).FirstOrDefault();

            Productlist product = GetProduct(productId);
            product.InCart = true;
            _genericRepository.Update(product);
            CartProducts cartitem = new CartProducts();
            cartitem.Count = 1;
            //cartitem.User.Id = userId;
            //cartitem.product.Id= productId;
            cartitem.User = query;
            cartitem.product = productquery;
            db.CartProducts.Add(cartitem);
            db.SaveChanges();


        }
       

        /// <summary>
        /// Search the product by Passing the search string which check conatins in product name and description
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public IEnumerable<Productlist> GetSearchItems(string searchString)
        {
            List<Productlist> productlist = new List<Productlist>();
            _genericRepository.GetAll().ToList().ForEach(u =>
            {
                Productlist product = null;
                product = new Productlist()
                {
                    Id = u.Id,
                    ProductDescription = u.ProductDescription,
                    ProductPrice = u.ProductPrice,
                    ProductName = u.ProductName,
                    ProductImage = u.ProductImage,
                    InStock = u.InStock,
                    InCart = u.InCart,
                    Quantity = u.Quantity,
                };
                if (product.ProductName.Contains(searchString.ToUpper()) || product.ProductDescription.Contains(searchString.ToUpper()))
                {
                    productlist.Add(product);
                }
            });
            IEnumerable<Productlist> products = productlist;
            return products;
        }
        public string CheckingRole(long userid)
        {
            if (userid == 0)
            {
                return "";
            }
            return db.Register.Where(c => c.Id == userid).FirstOrDefault().Role;
        }

        public void ResettingAddtoCart(long currentuserid)
        {
            var removeIncart = (from pl in db.Productlist
                                select pl).ToList();
            foreach (var item in removeIncart)
            {
                item.InCart = false;
                db.Productlist.Update(item);
            }

            //Need to pass the User Id here
            var updateIncart = (from cp in db.CartProducts
                                where cp.User.Id == currentuserid
                                select cp).ToList();
            foreach (var item in updateIncart)
            {
                var pItem = db.Productlist.Where(pl => pl.Id == item.product.Id).FirstOrDefault();
                pItem.InCart = true;
                db.Productlist.Update(pItem);
            }
            db.SaveChanges();
        }


    }
}
