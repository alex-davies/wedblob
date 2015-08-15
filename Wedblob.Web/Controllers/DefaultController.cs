using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wedblob.Web.Infrastructure;
using Wedblob.Web.Models;
using Wedblob.Web.Services;

namespace Wedblob.Web.Controllers
{
    public class DefaultController : BaseController
    {
        public readonly IContentService _contentService;
        public readonly Hashids _hashids;
        public readonly ISettings _settings;

        public DefaultController(ISettings settings, IContentService contentService, Hashids hashids)
        {
            _contentService = contentService;
            _hashids = hashids;
            _settings = settings;
        }

        public async Task<ActionResult> Index(string tag)
        {
            List<string> permission = new List<string>();
            if (string.IsNullOrEmpty(_settings.RSVPKeyword) || _settings.RSVPKeyword.Equals(tag, StringComparison.OrdinalIgnoreCase))
            {
                permission.Add("rsvp");
            }

            var model = new RootContentFragment(_contentService.GetContentRoot(), permission);
            return View(model.DataType, model);
        }
    }
}