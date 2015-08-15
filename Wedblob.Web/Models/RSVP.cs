using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wedblob.Web.Models
{
    public class RSVP
    {
        public string Id { get; set; }

        public string Tag { get; set; }

        public bool? Attending { get; set; }

        public string[] Guests { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RSVP()
        {
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }
    }
}