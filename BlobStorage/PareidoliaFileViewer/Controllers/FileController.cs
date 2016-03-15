using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PareidoliaFileViewer.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        // GET: api/File
        public IEnumerable<string> Get()
        {
            List<string> blobs = new List<string>();

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("files");
            foreach(var item in container.ListBlobs(null, false))
            {
                var blob = (CloudBlockBlob)item;
                blobs.Add(blob.Uri.AbsolutePath);
            }

            return blobs.ToArray();
        }

        // GET: api/File/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/File
        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("files");

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (var file in provider.FileData)
                {
                    var fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                    var blob = container.GetBlockBlobReference(fileName);

                    // Set the blob content type
                    blob.Properties.ContentType = file.Headers.ContentType.MediaType;

                    // Upload file into blob storage, basically copying it from local disk into Azure
                    using (var fs = File.OpenRead(file.LocalFileName))
                    {
                        blob.UploadFromStream(fs);
                    }

                    // Delete local file from disk
                    File.Delete(file.LocalFileName);
                }
                return Ok();
            }
            catch (System.Exception e)
            {
                return InternalServerError(e);
            }
        }

        // PUT: api/File/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/File/5
        public void Delete(int id)
        {
        }
    }
}
