using ProiectDaw.Models;
using Project_plus_Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ProiectDaw.Models.Product;

namespace ProiectDaw.Controllers
{
//[Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {

        private ApplicationDbContext db = ApplicationDbContext.Create();
        private static Random random = new Random();
        // GET: Category

        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            return View();
        }


        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            category.Products = GetProducts(id);

            if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            return View(category);
        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New()
        {
            return View();
        }

        
        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New(Category cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (cat.Image != null)
                    {
                        var filename = string.Concat(RandomString(10), cat.Image.FileName);
                        var path = string.Concat("~/Images/", filename);
                        cat.Image.SaveAs(Server.MapPath("~/Images/") + filename);
                        cat.ImagePath = path;
                    }

                    db.Categories.Add(cat);
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost adaugata!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cat);
                }
            }
            catch (Exception e)
            {
                return View(cat);
            }
        }

       [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        
        [HttpPut]
       [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id, Category requestCategory)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.CategoryName = requestCategory.CategoryName;
                        category.CategoryDescription = requestCategory.CategoryDescription;
                       
                        TempData["message"] = "Categoria a fost modificata!";
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }      
            }
            catch (Exception e)
            {
                return View(requestCategory);
            }
          
        }

        [HttpDelete]
      [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            TempData["message"] = "Categoria a fost stearsa!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        public List<Product> GetProducts(int CategoryId)
        {
            // generam o lista goala
            var selectList = new List<Product>();

            // Extragem toate categoriile din baza de date
            var products = from prod in db.Products where (prod.CategoryId == CategoryId)
                             select prod;


            // iteram prin categorii
            foreach (var product in products)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                if(product.ProductAprove == true)
                {
                    selectList.Add(product);
                }
              
            }



            // returnam lista de categorii
            return selectList;
        }

        [NonAction]
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}