using ImageMagick;
using Microsoft.WindowsAzure.Storage.Blob;
using PareidoliaFileViewer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumnailRole
{
    public class ImageProcessor
    {
        public static Task<string> CreateThumbnail(CloudBlobContainer imgContainer, CloudBlobContainer thumbContainer, IRedisProvider redis, string imageName)
        {
            return Task.Factory.StartNew<string>(() =>
            {
                CloudBlockBlob imageBlob = imgContainer.GetBlockBlobReference(imageName);

                string extension = imageName.Split('.')[1];
                string thumbnailName = "";

                using (var memoryStream = new MemoryStream())
                {
                    imageBlob.DownloadToStream(memoryStream);
                    using (MagickImage image = new MagickImage(memoryStream))
                    {
                        thumbnailName = Guid.NewGuid().ToString() + "." + extension;
                        CloudBlockBlob thumbBlob = thumbContainer.GetBlockBlobReference(thumbnailName);
                        thumbBlob.Properties.ContentType = imageBlob.Properties.ContentType;

                        MagickGeometry size = new MagickGeometry(100, 100);
                        // This will resize the image to a fixed size without maintaining the aspect ratio.
                        // Normally an image will be resized to fit inside the specified size.
                        size.IgnoreAspectRatio = true;

                        image.Resize(size);
                        
                        byte[] imageArray = image.ToByteArray(MagickFormat.Jpg);
                        thumbBlob.UploadFromByteArray(imageArray, 0, imageArray.Length);

                        redis.UpdateThumbnailImageUrl(imageName, thumbBlob.Uri.AbsoluteUri);
                    }
                }
                
                return thumbnailName;
            });
        }
    }
}
