using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KbiApps.Orchard.MediaApi.Services
{
    public class PathService : IPathService
    {
        public string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}