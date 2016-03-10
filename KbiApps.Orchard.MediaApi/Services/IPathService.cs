using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KbiApps.Orchard.MediaApi.Services
{
    public interface IPathService
    {
        string MapPath(string path);
    }
}
