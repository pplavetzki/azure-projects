using PareidoliaFileViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PareidoliaFileViewer.Services.Interfaces
{
    public interface IRedisProvider
    {
        Task AddToImages(Image image);
        Task<IEnumerable<Image>> GetImages(long start = 0, long end = -1);
    }
}
