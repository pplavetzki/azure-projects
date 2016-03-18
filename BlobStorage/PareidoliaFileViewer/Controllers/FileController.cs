using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PareidoliaFileViewer.Models;
using PareidoliaFileViewer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace PareidoliaFileViewer.Controllers
{ 
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        private readonly ISASTokenProvider _sasTokenProvider;
        private IRedisProvider _redisProvider;
        private readonly IQueueProvider _queueProvider;

        public FileController(ISASTokenProvider sasTokenProvider, IRedisProvider redisProvider, IQueueProvider queueProvider)
        {
            _sasTokenProvider = sasTokenProvider;
            _redisProvider = redisProvider;
            _queueProvider = queueProvider;
        }

        [HttpGet]
        [ResponseType(typeof(SASResponse))]
        [Route("SASToken")]
        public IHttpActionResult GetSASToken(string fileName)
        {
            var extension = fileName.Split('.');
            Regex rg = new Regex(@"^[a-zA-Z0-9]{1,3}$");

            if (extension.Count() != 2 || !rg.IsMatch(extension[1]))
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
            }

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("files");
            if(!container.Exists())
            {
                container.Create();
            }
            
            var id = Guid.NewGuid().ToString();
            var newFileName = id + "." + extension[1];

            var blob = container.GetBlockBlobReference(newFileName);

            var response = new SASResponse();
            response.SASToken = _sasTokenProvider.GetSASToken(container);
            response.BlobUrl = blob.Uri.AbsoluteUri;
            response.Id = id;
            response.FileName = newFileName;
           
            return Ok(response);
        }

        // GET: api/File
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var images = await _redisProvider.GetImages();

            return Ok(images);
        }

        // GET: api/File/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/File
        [HttpPost]
        public async Task<IHttpActionResult> Post(Image image)
        {
            try
            {
                await _redisProvider.AddImage(image);
                await _redisProvider.AddToImages(image);
                await _queueProvider.AddThumbMessage(image.BlobName);

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
