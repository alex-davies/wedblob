using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Wedblob.Web.Infrastructure.Extensions;

namespace Wedblob.Web.Models
{
    public class RootContentFragment : ContentFragment
    {
        public RootContentFragment(dynamic root, IEnumerable<string> permissions = null)
        {
            this.Root = root;
            this.Data = root;
            this.Permissions = permissions ?? Enumerable.Empty<string>();
        }
    }

    public class ContentFragment
    {
        public dynamic Root { get; set; }

        public dynamic Data { get; set; }

        public IEnumerable<string> Permissions { get; set; }

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
                Data = subFragmentData,
                Permissions = this.Permissions
            };
        }

        public bool HasPermission(dynamic data = null)
        {
            data = data ?? Data;
            var permissions = this.Permissions ?? Enumerable.Empty<string>();

            var requiredPermissions = data.requiredPermissions;
            if (requiredPermissions == null)
                return true;

            var requiredPermissionsString = requiredPermissions as string;
            if (requiredPermissionsString != null)
            {
                return permissions.Contains(requiredPermissionsString);
            }

            var requiredPermissionsList = requiredPermissions as IEnumerable;
            if (requiredPermissionsList != null)
            {
                return requiredPermissionsList.OfToStrings().All(x => permissions.Contains(x));
            }

            return false;
        }
    }
}