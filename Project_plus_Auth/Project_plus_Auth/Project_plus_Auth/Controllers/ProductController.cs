using Microsoft.AspNet.Identity;
using ProiectDaw.Models;
using Project_plus_Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectDaw.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        private static Random random = new Random();

       // [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Index()
        {

            var products = from prod in db.Products
                          where prod.ProductAprove == true
                          select prod;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.Products = products;

            return View();
        }

      //[Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Show(int id)
        {
            Product product = db.Products.Find(id);

            if (User.IsInRole("Editor") || User.IsInRole("Administrator") || User.IsInRole("User"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            
            ViewBag.Comments = GetAllComments(id);
            ViewBag.Stars = GetReview(id);

            return View(product);
        }

     [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New()
        {
            Product product = new Product();

            product.Categories = GetAllCategories();

            product.UserId = User.Identity.GetUserId();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New(Product product)
        {
            product.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    if (product.Image != null)
                    {
                        var filename = string.Concat(RandomString(10), product.Image.FileName);
                        var path = string.Concat("~/Images/", filename);
                        product.Image.SaveAs(Server.MapPath("~/Images/") + filename);
                        product.ImagePath = path;
                    }
                    //aprove
                    product.ProductAprove = false;
                    
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(product);
                }
            }
            catch (Exception e)
            {
                return View(product);
            }
        }

        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Edit(int id)
        {
            Product product = db.Products.Find(id);
            ViewBag.Product = product;
            product.Categories = GetAllCategories();

            if (product.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Edit(int id, Product requestProduct)
        {
            requestProduct.Categories = GetAllCategories();
            System.Diagnostics.Debug.WriteLine("alo  ");
            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(id);
                    if(product.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(product))
                        {

                            if (requestProduct.Image != null) // nu intra aici
                            {

                                System.Diagnostics.Debug.WriteLine("alo  ");
                                var filename = string.Concat(RandomString(10), requestProduct.Image.FileName);
                                var path = string.Concat("~/Images/", filename);
                                requestProduct.Image.SaveAs(Server.MapPath("~/Images/") + filename);
                                requestProduct.ImagePath = path;

                              
                            }

                            //product.ImagePath = requestProduct.ImagePath;
                            product.ImagePath = product.ImagePath;
                            product.Image = product.Image;

                            //cantitatea
                            product.orderQuantity = product.orderQuantity;

                            product.ProductName = requestProduct.ProductName;
                            product.ProductPrice = requestProduct.ProductPrice;
                            product.ProductDescription = requestProduct.ProductDescription;
                            //product.ProductPhoto = requestProduct.ProductPhoto;
                            //product.ProductRating = requestProduct.ProductRating;
                            
                            product.CategoryId = requestProduct.CategoryId;
                            //product.ProductMark = requestProduct.ProductMark;
                            db.SaveChanges();
                            TempData["message"] = "Produsul a fost modificat!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                       TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";
                       return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestProduct);
                }

            }
            catch (Exception e)
            {
                return View(requestProduct);
            }
           
        }

        [HttpDelete]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);
            if (product.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                db.Products.Remove(product);
                bool b = this.DeleteReviews(id);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs care nu va apartine!";
                return RedirectToAction("Index");
            }
            
        }

        [NonAction]
        // [Authorize(Roles = "Editor,Administrator")]
        public bool DeleteReviews(int id)
        {
            Product product = db.Products.Find(id);
            var reviews = from rev in db.Ratings
                          where rev.ProductId == id
                          select rev;
           
            foreach (StarRating rev in reviews)
            {
                db.Ratings.Remove(rev);
            }
            db.SaveChanges();
            return true;
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }

       


        public ActionResult OnPress(int id)
        {
            string userid = User.Identity.GetUserId();

            Product product = db.Products.Find(id);
           
            ShoppingList cartList = db.ShoppingLists.Find(userid);
            System.Diagnostics.Debug.WriteLine(id + "----id");
            if (cartList == null)
            {
                System.Diagnostics.Debug.WriteLine("nu a gasit" + product.ProductName);
                cartList = new ShoppingList();
                cartList.ShoppingListId = userid;
                cartList.List = new List<Product>();
                cartList.List.Add(product);
                db.ShoppingLists.Add(cartList);
                TempData["message"] = "Produsul a fost adaugat in cos!";
                db.SaveChanges();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("a gasit" + product.ProductName);
                if (this.CheckProduct(cartList.List,product.ProductName))
                {
                    
                    TempData["message"] = "Produsul este deja cos!";
                }
                else
                {
                    cartList.List.Add(product);
                    product.orderQuantity++;
                    TempData["message"] = "Produsul a fost adaugat in cos!";
                }
                
                db.SaveChanges();
            }
            

            db.SaveChanges();
         
            return RedirectToAction("Index");
        }

        public ActionResult OnPressAddComment(int id)
        {
            Product product = db.Products.Find(id);
          

            return RedirectToAction("New","Comment",new {id = id });
        }

        public ActionResult Shopping()
        {
            return View();
        }
       

        [NonAction]
        public List<string> GetAllComments(int productId)
        {
            List<string> commentsList = new List<string>();
            var comments = from comm in db.CommentLists
                           where comm.CommentProductId == productId
                           select comm;
         
            foreach (var comm in comments)
            {
                commentsList.Add(comm.CommentTxt);
            }
            return commentsList;
        }

        [NonAction] // returneaza true daca l a gasit
        public bool CheckProduct(List<Product> l,string numeProdus)
        {
            foreach(var prod in l)
            {
                if (prod.ProductName == numeProdus)
                    return true;
            }
            return false;
        }

        [NonAction]
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

       
        public ActionResult OnPressStar(int id,int stars)
        {
            System.Diagnostics.Debug.WriteLine("id" + id + "stele : "+ stars );
            //verifica daca produsul are deja review de la user
            Product product = db.Products.Find(id);
            StarRating review = new StarRating();
            var reviews = from rev in db.Ratings
                           where rev.ProductId == id
                           && rev.UserId == product.UserId
                           select rev;
            var check = 0;
            foreach(StarRating rev in reviews)
            {
                check++;
                review = rev;

            }
            if(check == 0)// nu exista review de la user si trb creat
            {
                System.Diagnostics.Debug.WriteLine("nu exista");
                review.ProductId = product.ProductId;
                review.UserId = product.UserId;
                review.Rate = stars;

                db.Ratings.Add(review);
                db.SaveChanges();

            }
            else//exista review si trb modificat
            {
                System.Diagnostics.Debug.WriteLine(" exista");
                review.Rate = stars;
                db.SaveChanges();
            }

            return RedirectToAction("Show", "Product",new { id = product.ProductId});
        }

        [NonAction]
        public int GetReview(int id)
        {
            // generam o lista goala
            int stars = 0;
            Product product = db.Products.Find(id);
            // Extragem toate categoriile din baza de date
            var reviews = from rev in db.Ratings
                          where rev.ProductId == product.ProductId
                          where rev.UserId == product.UserId
                          select rev;

            // iteram prin categorii
            foreach(StarRating r in reviews)
            {
                stars = r.Rate;
            }

            // returnam lista de categorii
            return stars;
        }

        public ActionResult Search(string searchVal,string sortOrder)
        {
            System.Diagnostics.Debug.WriteLine( "sort    " + sortOrder);
            if (searchVal == "" || searchVal == null)
            { 
                switch (sortOrder)
                {
                    case "Asc Price":
                        ViewBag.List = db.Products.ToList().OrderBy(p => p.ProductPrice).Where(p => p.ProductAprove == true);
                        break;
                    case "Desc Price":
                        ViewBag.List = db.Products.ToList().OrderByDescending(p => p.ProductPrice).Where(p => p.ProductAprove == true);
                        break;
                    case "Asc Rating":
                        ViewBag.List = db.Products.ToList().OrderBy(p => p.Rating).Where(p => p.ProductAprove == true);
                        break;
                    case "Desc Rating":
                        ViewBag.List = db.Products.ToList().OrderByDescending(p => p.Rating).Where(p => p.ProductAprove == true);
                        break;
                    default:
                        ViewBag.List = db.Products.ToList().Where(p => p.ProductAprove == true);
                        break;
                }
            }
            else
            {
                var list = from prod in db.Products
                           where prod.ProductName.Contains(searchVal)
                           select prod;

                switch (sortOrder)
                {
                    case "Asc Price":
                        ViewBag.List = list.ToList().OrderBy(p => p.ProductPrice).Where(p => p.ProductAprove == true);
                        break;
                    case "Desc Price":
                        ViewBag.List = list.ToList().OrderByDescending(p => p.ProductPrice).Where(p => p.ProductAprove == true);
                        break;
                    case "Asc Rating":
                        ViewBag.List = list.ToList().OrderBy(p => p.Rating).Where(p => p.ProductAprove == true);
                        break;
                    case "Desc Rating":
                        ViewBag.List = list.ToList().OrderByDescending(p => p.Rating).Where(p => p.ProductAprove == true);
                        break;
                    default:
                        ViewBag.List = list.ToList().Where(p => p.ProductAprove == true);
                        break;
                }
            }
         
            return View();
        }

        public ActionResult Aprove()
        {
            var products = from prod in db.Products
                           where prod.ProductAprove == false
                           select prod;

            ViewBag.Products = products;
            return View();
        }

        public ActionResult AproveProduct(int id)
        {
            Product product = db.Products.Find(id);
            product.ProductAprove = true;
            db.SaveChanges();
            TempData["message"] = "Produsul a fost aprobat!";
            return RedirectToAction("Index", "Product");
        }

     

    }
}
