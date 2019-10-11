using ImageGallery.Data;
using ImageGallery.Data.Interfaces;
using ImageGallery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class ImageService : IImage
    {
        private readonly ImageGalleryDbContext _context;
        public ImageService(ImageGalleryDbContext context)
        {
            _context = context;
        }

        public IEnumerable<GalleryImage> GetAll()
        {
            var result = _context.GalleryImages
                .Include(img => img.Tags);

            return result;
        }

        public GalleryImage GetById(int id)
        {
            return GetAll().Where(img => img.Id == id).First();
        }

        public IEnumerable<GalleryImage> GetByTag(string tag)
        {
            return GetAll()
                .Where(img => img.Tags
                .Any(t => t.Title == tag));
        }
        
        public CloudBlobContainer GetBlobContainer(string azureConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        public async Task SetImage(string title, string tags, Uri uri)
        {
            // Check tags is null or isn't
            List<ImageTag> listTags = null;
            if(tags == null)
            {
                listTags = ParseTags(tags);
            }

            var image = new GalleryImage()
            {
                Title = title,
                Tags = listTags,
                Url = uri.AbsoluteUri,
                Created = DateTime.Now,
            };

            _context.Add(image);
            await _context.SaveChangesAsync();
        }

        public List<ImageTag> ParseTags(string tags)
        {
            var tagList = tags.Split(",")
                .Select(tag => new ImageTag { Title = tag });

            //var imageTags = new List<ImageTag>();
            //foreach (var tag in tagList)
            //{
            //    imageTags.Add(new ImageTag
            //    {
            //        Title = tag
            //    });
            //}

            return tagList.ToList();
        }
    }
}
