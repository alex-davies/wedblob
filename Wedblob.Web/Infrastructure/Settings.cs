using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Wedblob.Web.Infrastructure
{
    public interface ISettings
    {
        string MongoDBConnectionString { get; }
        string MongoDBName { get; }
        string HashIdSalt { get; }
        string HashIdAlphabet { get; }
        int HashIdMinLength { get; }
        string RSVPKeyword { get; }
    }

    public class Settings : ISettings
    {
        public string MongoDBConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
            }
        }

        public string MongoDBName
        {
            get
            {
                var index = MongoDBConnectionString.LastIndexOf('/');
                return MongoDBConnectionString.Substring(index+1);
            }
        }

        public string HashIdSalt
        {
            get
            {
                return ConfigurationManager.AppSettings["HashId.Salt"];
            }
        }

        public string HashIdAlphabet
        {
            get
            {
                return ConfigurationManager.AppSettings["HashId.Alphabet"];
            }
        }

        public int HashIdMinLength
        {
            get
            {
                int minLength;
                if (!int.TryParse(ConfigurationManager.AppSettings["HashId.MinLength"], out minLength))
                    minLength = 0;
                return minLength;
            }
        }

        public string RSVPKeyword
        {
            get
            {
                return ConfigurationManager.AppSettings["RSVP.Keyword"];
            }
        }
    }
}