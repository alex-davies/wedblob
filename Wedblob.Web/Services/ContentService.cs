using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Wedblob.Web.Services
{
    public interface IContentService
    {
        dynamic GetContentRoot();
    }

    public class ContentService : IContentService
    {
        public dynamic GetContentRoot()
        {

            string path = HostingEnvironment.MapPath("~/App_Data/content.json");
            // deserialize JSON directly from a file
            using (var fileReader = File.OpenText(path))
            using (var jsonTextReader = new JsonTextReader(fileReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<dynamic>(jsonTextReader);
            }
        }
    }
}