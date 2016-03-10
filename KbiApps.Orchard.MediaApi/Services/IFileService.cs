using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KbiApps.Orchard.MediaApi.Services
{
    public interface IFileService
    {
        bool Exists(string path);
        byte[] ReadAllBytes(string path);
    }
}
