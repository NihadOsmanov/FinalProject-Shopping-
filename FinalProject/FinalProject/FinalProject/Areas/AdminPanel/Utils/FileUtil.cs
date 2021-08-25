using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminPanel.Utils
{
    public static class FileUtil
    {
        public static async Task<string> GenerateFileAsync(string folderPath, IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}-{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }
    }
}
