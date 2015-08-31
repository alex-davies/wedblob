using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wedblob.Web.Models
{
    public class RSVP
    {
        public class Guest
        {
            public string Name { get; set; }

            public bool? Attending { get; set; }
        }

        public string Id { get; set; }

        public Guest[] Guests { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RSVP()
        {
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }
    }

    public class Rsvp
    {
        public int RsvpID { get; set; }

        public string GroupName {get; set;}

        public string Name { get; set;}

        public bool? Attending { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}