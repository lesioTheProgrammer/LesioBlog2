using LeisoBlog2_Repo.Abstract;
using System.Web.Mvc;

namespace LesioBlog2.Controllers
{
    public class TagsController : Controller
    {

        private readonly ITagRepo _tag;
        public TagsController(ITagRepo tagz)
        {
            this._tag = tagz;
        }


        // GET all postts matching tag included
        public ActionResult Index(string tagName)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                return RedirectToAction("Index", "Wpis");
            }
            else
            {
                var listOfWpisWithTags =  _tag.getWpisWithSelectedTag(tagName);
                return View(listOfWpisWithTags);
            }
        }
    }
}
