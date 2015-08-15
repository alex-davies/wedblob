using HashidsNet;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wedblob.Web.Models;
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
        private readonly Hashids _hashId;
        private readonly IDocumentStore _documentStore;

        public RSVPController(Hashids hashId, IDocumentStore documentStore)
        {
            this._hashId = hashId;
            this._documentStore = documentStore;
        }

        [HttpGet]
        public async Task<ActionResult> Index([Bind(Prefix = "$skip")] int? skip = null, [Bind(Prefix = "$top")]int? top = null)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                IQueryable<RSVP> rsvps = session.Query<RSVP>();
                if (skip != null)
                    rsvps = rsvps.Skip(skip.Value);
                if (top != null)
                    rsvps = rsvps.Take(top.Value);

 
                return Json(await rsvps.Select(rsvp => (new RSVPOutputModel()
                {
                    attending = rsvp.Attending,
                    guests = rsvp.Guests,
                    tag = rsvp.Tag
                })).ToListAsync());
            }

            
        }

        [HttpGet]
        public async Task<ActionResult> Get(string tag)
        {
            if(string.IsNullOrWhiteSpace(tag))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            using (var session = _documentStore.OpenAsyncSession())
            {
                RSVP rsvp = await session.Query<RSVP>().FirstOrDefaultAsync(x => x.Tag == tag);
                if (rsvp == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                

                return Json(new RSVPOutputModel()
                {
                    attending = rsvp.Attending,
                    guests = rsvp.Guests,
                    tag = rsvp.Tag
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(RSVPInputModel data)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var rsvp = new RSVP();
                await session.StoreAsync(rsvp);

                rsvp.Tag = data.tag ?? GenerateTagFromId(rsvp.Id);
                rsvp.Guests = data.guests;
                rsvp.Attending = data.attending;
                rsvp.Updated = DateTime.UtcNow;


                await session.SaveChangesAsync();

                return Json(new RSVPOutputModel()
                {
                    attending = rsvp.Attending,
                    guests = rsvp.Guests,
                    tag = rsvp.Tag
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(string tag, RSVPInputModel data)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                

            using (var session = _documentStore.OpenAsyncSession())
            {

                RSVP rsvp = await session.Query<RSVP>().FirstOrDefaultAsync(x => x.Tag == tag);
                if(rsvp == null)
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                rsvp.Guests = rsvp.Guests ?? data.guests;
                rsvp.Attending = rsvp.Attending ?? data.attending;
                rsvp.Tag = rsvp.Tag ?? data.tag;
                rsvp.Updated = DateTime.UtcNow;

                await session.SaveChangesAsync();

                return Json(new RSVPOutputModel()
                {
                    attending = rsvp.Attending,
                    guests = rsvp.Guests,
                    tag = rsvp.Tag
                });
            }
        }

        private string GenerateTagFromId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var intPartString = id.Substring(id.IndexOf('/')+1);
            int intPart;
            if (!int.TryParse(intPartString, out intPart))
                return null;
            return _hashId.Encode(intPart);
        }
    }
}