using ImageGallery.Data.Models;
using System.Collections.Generic;

namespace ImageGallery.Models
{
    public class GalleryIndexModel
    {
        public IEnumerable<GalleryImage> Images { get; set; }
        public string SearchQuery { get; set; }

    }
}
