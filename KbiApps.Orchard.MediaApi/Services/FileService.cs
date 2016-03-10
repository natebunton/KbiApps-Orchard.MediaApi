using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KbiApps.Orchard.MediaApi.Services
{
    public class FileService : IFileService
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}