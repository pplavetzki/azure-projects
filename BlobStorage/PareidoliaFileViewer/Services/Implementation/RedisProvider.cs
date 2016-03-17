using PareidoliaFileViewer.Models;
using PareidoliaFileViewer.Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PareidoliaFileViewer.Services.Implementation
{
    public class RedisProvider : IRedisProvider
    {
        private IConnectionMultiplexer _redis;

        public RedisProvider(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<string> GetThumbnailImageUrl(string fileName)
        {
            var image = await GetImage(fileName);

            return image.ThumbnailUrl;
        }

        public async Task AddImage(Image image)
        {
            var database = _redis.GetDatabase();
            string imageJson = await Task.Run(() => JsonConvert.SerializeObject(image));

            await database.StringSetAsync(image.Id, imageJson);
        }

        public async Task AddToImages(Image image)
        {
            var database = _redis.GetDatabase();
            string imageJson = await Task.Run(() => JsonConvert.SerializeObject(image));

            await database.ListLeftPushAsync("images", imageJson);
        }

        public async Task<IEnumerable<Image>> GetImages(long start, long end)
        {
            List<Image> images = new List<Image>();

            var database = _redis.GetDatabase();
            var values = await database.ListRangeAsync("images", start, end);

            foreach(var image in values)
            {
                images.Add(await Task.Run(() => JsonConvert.DeserializeObject<Image>(image)));
            }

            return images;
        }

        public async Task<Image> GetImage(string fileName)
        {
            var database = _redis.GetDatabase();
            var image = JsonConvert.DeserializeObject<Image>(await database.StringGetAsync(fileName));

            return image;
        }

        public async Task UpdateThumbnailImageUrl(string fileName, string thumbnailUrl)
        {
            var image = await GetImage(fileName);
            image.ThumbnailUrl = thumbnailUrl;

            await AddImage(image);
        }
    }
}
