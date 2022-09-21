using Deal.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Services
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _IWebHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _IWebHostEnvironment = webHostEnvironment;
        }

        public string ProductImageUpload(Product product)
        {
            string folder = "ProductImages";
            string fileName = product.ProductId.ToString();
            return UploadedFile(product.ImageFile, folder, fileName);
        }
        public string CustomPaymentFile(CustomPayment payment)
        {
            string folder = "CustomPaymentFiles";
            string fileName = payment.Id.ToString();
            return UploadedFile(payment.File, folder, fileName);
        }
        

        private string UploadedFile(IFormFile file, string folder,string fileName)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                var fileExt = file.FileName.Substring(file.FileName.LastIndexOf('.')); // get file extention

                string uploadsFolder = Path.Combine(_IWebHostEnvironment.WebRootPath, folder);
                uniqueFileName = fileName + fileExt; // file rename
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                uniqueFileName = "/" + folder + "/" + fileName+fileExt;
            }
            return uniqueFileName;
        }
    }
}
