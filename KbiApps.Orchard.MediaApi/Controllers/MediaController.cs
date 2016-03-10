using KbiApps.Orchard.MediaApi.Models;
using KbiApps.Orchard.MediaApi.Services;
using Orchard.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web;
using System.Web.Http;

namespace KbiApps.Orchard.MediaApi.Controllers
{
    public class MediaController : ApiController
    {
        private const string DefaultAllowedExtensions = ".png, .jpg, .jpeg, .gif, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .pdf";
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        public ILogger Logger { get; set; }

        public MediaController()
        {
            _fileService = new FileService();
            _pathService = new PathService();
            Logger = NullLogger.Instance;
        }

        public MediaController(IFileService fileService, IPathService pathService)
        {
            _fileService = fileService;
            _pathService = pathService;
        }

        public HttpResponseMessage Get(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    Logger.Warning("Url cannot be null or empty.");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request - Url cannot be null or empty");
                }

                var fullPath = _pathService.MapPath(url);

                // Check if file extension is valid
                if (!IsExtensionAllowed(fullPath))
                {
                    Logger.Warning(string.Format("Unsupported Media Type - File Extension Not Allowed.  Url: {0}", url));
                    return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported Media Type - File Extension Not Allowed");
                }

                // TODO: Check if path is local

                // Check if file exists
                if (!_fileService.Exists(fullPath))
                {
                    Logger.Warning(string.Format("File Not Found.  Url: {0}", url));
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found - File Not Found");
                }

                // Get Bytes
                byte[] byteArray;
                try
                {
                    byteArray = _fileService.ReadAllBytes(fullPath);
                }
                catch (SecurityException securityException)
                {
                    Logger.Error(securityException, string.Format("A security exception occurred getting media.  Url: {0}", url));
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized - Access Denied to file");
                }

                // Get Mime Type
                var mimeType = MimeMapping.GetMimeMapping(url);

                var media = new Media
                {
                    MimeType = mimeType,
                    Data = byteArray
                };

                return Request.CreateResponse(HttpStatusCode.OK, media);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("An unexpected exception occurred getting media.  Url: {0}", url));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        private static bool IsExtensionAllowed(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return false;
            }

            var extension = Path.GetExtension(fullPath);

            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            //TODO: Get allowed extensions from admin
            var allowedExtensionsConfiguration = string.Empty;

            // If allowed extensions configuration is not supplied, use the default allowed extensions
            if (string.IsNullOrEmpty(allowedExtensionsConfiguration))
            {
                allowedExtensionsConfiguration = DefaultAllowedExtensions;
            }

            var allowedExtensionsArray = allowedExtensionsConfiguration.Replace(" ", "").ToLower().Split(',');
            return allowedExtensionsArray.Contains(extension.ToLower());
        }
    }
}
