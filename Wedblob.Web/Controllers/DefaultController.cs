using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wedblob.Web.Models;
using Wedblob.Web.Services;

namespace Wedblob.Web.Controllers
{
    public class DefaultController : Controller
    {
        public readonly IContentService ContentService;

        public DefaultController(IContentService contentService)
        {
            ContentService = contentService;
        }

        // GET: Default
        public ActionResult Index()
        {
            var model = new RootContentFragment(ContentService.GetContentRoot());

            return View(model.DataType, model);
        }
    }
}