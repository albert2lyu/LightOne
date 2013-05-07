using System;
using System.Collections.Generic;
using System.Linq;
using Business;
using System.Web.Mvc;

namespace Api.Controllers
{
    public class CategoriesController : Controller
    {
        [HttpPost]
        public ActionResult Upsert(IEnumerable<Category> categories) {
            var needProcessCategories = Category.Upsert(categories);
            return new Api.Models.JsonResult(needProcessCategories);
        }
    }
}
