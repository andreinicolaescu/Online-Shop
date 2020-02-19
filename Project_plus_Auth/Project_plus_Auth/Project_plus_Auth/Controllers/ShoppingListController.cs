using Microsoft.AspNet.Identity;
using ProiectDaw.Models;
using Project_plus_Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_plus_Auth.Controllers
{
    [Authorize(Roles = "User,Editor,Administrator")]
    public class ShoppingListController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: ShoppingList
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            ShoppingList list = db.ShoppingLists.Find(id);

            int totalPrice = 0;
            
            foreach(Product product in list.List)
            {
                totalPrice = totalPrice + product.ProductPrice * product.orderQuantity;
            }
            ViewBag.l = list.List;
            ViewBag.totalPrice = totalPrice;
            return View();
            
        }
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult OnPressRemoveProduct(int id)
        {
            string userid = User.Identity.GetUserId();
            Product product = db.Products.Find(id);
            product.orderQuantity = 0;
            ShoppingList cartList = db.ShoppingLists.Find(userid);

            cartList.List.Remove(product);
            TempData["message"] = "Produsul a fost scos din cos!";

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult OnPressIncrement(int id)
        {
            Product product = db.Products.Find(id);
            product.orderQuantity++;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult OnPressDecrement(int id)
        {
            Product product = db.Products.Find(id);
            if(product.orderQuantity > 1)
            {
                product.orderQuantity--;
            }
            else
            {
                product.orderQuantity = 0;
                this.OnPressRemoveProduct(id);
            }
            
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}