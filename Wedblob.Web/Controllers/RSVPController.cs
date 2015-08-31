using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wedblob.Web.Models;
using Wedblob.Web.Services;
using Dapper;

namespace Wedblob.Web.Controllers
{
    public class GuestModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool? attending { get; set; }
    }

    public class RSVPSearchOutputModel
    {
        public string groupName { get; set; }
        public GuestModel[] guests { get; set; }
    }

    public class RSVPInputModel
    {
        public GuestModel[] guests { get; set; }
    }

    public class RSVPOutputModel
    {
        public GuestModel[] guests { get; set; }
    }



    public class RSVPController : BaseController
    {
        private readonly Database _db;

        public RSVPController(Database db)
        {
            this._db = db;
        }


        [HttpGet]
        public async Task<ActionResult> Index([Bind(Prefix = "$top")]int? top = null, string q = null)
        {
            //we dont want to worry about punctuation or numbers. Also protects against
            //some inputting in SQL wildcard characters (like '%')
            q = Regex.Replace(q, @"[^\p{L} ]", "");

            var result = await _db.Execute(async conn =>
            {
                var queryBase = @"SELECT * FROM Rsvp WHERE GroupName IN (SELECT GroupName FROM Rsvp WHERE {0});";

                var queryParams = new DynamicParameters();
                queryParams.Add("query", q);

                //our partial match has to have matching word for every query word
                int i = 0;
                var partialMatchWhereElements = new List<string>();
                foreach(var partialQ in q.Split(' '))
                {
                    var paramName = "partialQuery" + i++;
                    queryParams.Add(paramName, "% " + partialQ.Trim() + "%");
                    partialMatchWhereElements.Add("CONCAT(' ',Name,' ',ISNULL(AlternateNames,'')) LIKE @" + paramName);
                }

                //we will do one sql query for both an exact match and a partial match
                var multiSql = string.Join(Environment.NewLine,
                    string.Format(queryBase, @"Name LIKE @query"),
                    string.Format(queryBase, string.Join(" AND ", partialMatchWhereElements)));
                using (var multi = await conn.QueryMultipleAsync(multiSql, queryParams))
                {
                    //if we have an exact match use that, otherwise we resort ot accepting parital matches
                    var exactMatch = await multi.ReadAsync<Rsvp>();
                    if (exactMatch.Any())
                        return exactMatch;
                    return await multi.ReadAsync<Rsvp>();
                }
            });
            

            var output = result.GroupBy(x => x.GroupName).Select(x => new RSVPSearchOutputModel()
            {
                groupName = x.Key,
                guests = x.Select(g => new GuestModel()
                {
                    id = g.RsvpID,
                    name = g.Name,
                    attending = g.Attending

                }).ToArray()
            });
            if (top.HasValue)
                output = output.Take(top.Value);
            return Json(output.ToList());
        }

       
        [HttpPost]
        public async Task<ActionResult> Post(RSVPInputModel data)
        {
            var result = await _db.Execute(async conn =>
            {
     
                await conn.ExecuteAsync("UPDATE Rsvp SET Attending=@attending, Name=ISNULL(@name,Name), UpdatedDate=getdate() WHERE RsvpID=@id", data.guests.Select(x => new
                {
                    id = x.id,
                    attending = x.attending,
                    name = x.name
                }).ToArray());

                return await conn.QueryAsync<Rsvp>("SELECT * FROM Rsvp WHERE GroupName IN (SELECT GroupName FROM Rsvp WHERE RsvpID IN @ids)", new
                {
                    ids = data.guests.Select(x => x.id).ToArray()
                });
            });


            var output = result.GroupBy(x => x.GroupName).Select(x => new RSVPSearchOutputModel()
            {
                groupName = x.Key,
                guests = x.Select(g => new GuestModel()
                {
                    id = g.RsvpID,
                    name = g.Name,
                    attending = g.Attending

                }).ToArray()
            });
            return Json(output);

       }
       
    }

    
}