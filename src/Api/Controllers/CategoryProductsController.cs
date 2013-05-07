using System;
using System.Linq;
using Business;
using System.Web.Mvc;

namespace Api.Controllers
{
    public class CategoryProductsController : Controller
    {
        [HttpPost]
        public ActionResult Upsert(CategoryProducts categoryProducts) {
            categoryProducts.Upsert();
            return new EmptyResult();
        }
    }
}
