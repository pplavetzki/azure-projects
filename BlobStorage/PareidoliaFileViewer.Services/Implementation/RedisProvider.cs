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

            await database.StringSetAsync("image:" + image.Id, imageJson);
        }

        public async Task AddToImages(Image image)
        {
            var database = _redis.GetDatabase();
            string imageJson = await Task.Run(() => JsonConvert.SerializeObject(image));

            var length = await database.ListRightPushAsync("images", imageJson);
            var index = --length;

            await database.StringSetAsync("index:" + image.Id, index);
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
            var id = fileName.Split('.')[0];

            var database = _redis.GetDatabase();
            var image = JsonConvert.DeserializeObject<Image>(await database.StringGetAsync("image:" + id));

            return image;
        }

        public async Task UpdateThumbnailImageUrl(string fileName, string thumbnailUrl)
        {
            var database = _redis.GetDatabase();

            var image = await GetImage(fileName);
            image.ThumbnailUrl = thumbnailUrl;

            string imageJson = await Task.Run(() => JsonConvert.SerializeObject(image));

            await AddImage(image);

            var index = await database.StringGetAsync("index:" + image.Id);
            await database.ListSetByIndexAsync("images", Convert.ToInt64(index), imageJson);
        }
    }
}
