using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CulinaryCart.CulinaryFAL
{
    public interface IImageFAL
    {
     
        /// Save an uploaded image file and return its relative path.
        
        string SaveImage(IFormFile file);
        void DeleteImage(string filePath);

    }
}