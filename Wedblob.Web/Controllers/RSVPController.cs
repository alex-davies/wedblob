using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wedblob.Web.Services;

namespace Wedblob.Web.Controllers
{
    public class RSVPOutputModel
    {
        public string tag { get; set; }
        public bool? attending { get; set; }
        public string[] guests { get; set; }
    }

    public class RSVPInputModel
    {
        public string tag { get; set; }
        public bool? attending { get; set; }
        public string[] guests { get; set; }
    }

    public class RSVPController : BaseController
    {
        private readonly IRSVPService _rsvpService;
        private readonly Hashids _hashId;

        public RSVPController(Hashids hashId, IRSVPService rsvpService)
        {
            this._rsvpService = rsvpService;
            this._hashId = hashId;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var rsvps = await _rsvpService.FetchAll();
           
            return Json(rsvps.Select(rsvp=>(new RSVPOutputModel()
            {
                attending = rsvp.Attending,
                guests = rsvp.Guests,
                tag = rsvp.Id.HasValue ? _hashId.Encode(rsvp.Id.Value) : null
            })).ToList());
        }

        [HttpGet]
        public async Task<ActionResult> Get(string tag)
        {
            int id;
            try
            {
                var ids = _hashId.Decode(tag);
                if (ids.Length == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                id = ids[0];
            }
            catch (IndexOutOfRangeException ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var rsvp = await _rsvpService.Fetch(id);

            return Json(new RSVPOutputModel()
            {
                attending = rsvp.Attending,
                guests = rsvp.Guests,
                tag = rsvp.Id.HasValue ? _hashId.Encode(rsvp.Id.Value) : null
            });
        }

        [HttpPost]
        public async Task<ActionResult> Post(string tag, RSVPInputModel data)
        {
            int? id = null;
            if (tag != null)
            {
                var ids = _hashId.Decode(tag);
                if (ids.Length == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                id = ids[0];
            }

            var rsvp = await _rsvpService.Store(new RSVP()
            {
                Id = id,
                Guests = data.guests,
                Attending = data.attending
            });

            return Json(new RSVPOutputModel()
            {
                attending = rsvp.Attending,
                guests = rsvp.Guests,
                tag = rsvp.Id.HasValue ? _hashId.Encode(rsvp.Id.Value) : null
            });
        }
    }
}