using System;
using System.Collections.Generic;
using System.Linq;
using Business;
using System.Web.Mvc;

namespace Api.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryArchiveService _CategoryArchiveService = new CategoryArchiveService();

        [HttpPost]
        public ActionResult Upsert(IEnumerable<Category> categories) {
            var needProcessCategories = _CategoryArchiveService.Archive(categories);
            return new Api.Models.JsonResult(needProcessCategories);
        }
    }
}
