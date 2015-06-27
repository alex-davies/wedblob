using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Wedblob.Web.Models
{
    public class RootContentFragment : ContentFragment
    {
        public RootContentFragment(dynamic root)
        {
            this.Root = root;
            this.Data = root;
        }
    }

    public class ContentFragment
    {
        public dynamic Root { get; set; }

        public dynamic Data { get; set; }

        public string DataType{
            get{
                return Data.type;
            }
        }

        public ContentFragment SubFragment(dynamic subFragmentData)
        {
            return new ContentFragment()
            {
                Root = this.Root,
                Data = subFragmentData
            };
        }
    }
}