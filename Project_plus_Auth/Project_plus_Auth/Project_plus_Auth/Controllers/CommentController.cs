using Project_plus_Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_plus_Auth.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: CommentList
        public ActionResult Index(int id)
        {
            System.Diagnostics.Debug.WriteLine("controller commemnt cu produs id   " + id);
            return RedirectToAction("Index","Product");
        }
        public ActionResult New(int id)
        {
            Comment comment = new Comment();
            comment.CommentProductId = id;
            return View(comment);
        }

        [HttpPost]
        //  [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New(Comment comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.CommentLists.Add(comment);
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost adaugat!";
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    return View(comment);
                }
            }
            catch (Exception e)
            {
                return View(comment);
            }
        }

        public ActionResult ShowAll(int id)
        {

            List<Comment> commentsList = new List<Comment>();
            var comments = from comm in db.CommentLists
                           where comm.CommentProductId == id
                           select comm;

            foreach (var comm in comments)
            {
                commentsList.Add(comm);
            }

            ViewBag.Comments = commentsList;
            return View();
        }

        [HttpDelete]
        // [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(int id)
        {

            System.Diagnostics.Debug.WriteLine("stergere commemnt cu id   " + id);
            var comment = from comm in db.CommentLists
                           where comm.CommentId == id
                           select comm;

           foreach(Comment comm in comment)
            {
                db.CommentLists.Remove(comm);
            }

           
            db.SaveChanges();
            TempData["message"] = "Comentariul a fost sters!";
            return RedirectToAction("Index", "Product");

        }




    }
}