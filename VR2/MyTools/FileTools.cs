using Microsoft.AspNetCore.StaticFiles;
using System.Linq;
using VR2.DTOqMoels;
using System.Drawing;

namespace VR2.MyTools
{
    public class FileTools
    {
        public enum FileType
        {
            pdf,
            jpg,
            png,
            webp
        }
        public FileTools() { }

  private readonly List<string> _allowedImageExtensions = new() 
    { 
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp", ".svg" ,".pdf"
    };


        //

        public IFormFile GetIFormFile(string fileName)
        {
            var filePath = Path.Combine("wwwroot/uploads", fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var formFile = new FormFile(fileStream, 0, fileStream.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(filePath)
            };

            return formFile;
        }


        public List<IFormFile> GetIFormFiles(List<string> fileNames)
        {
            var files = new List<IFormFile>();

            foreach (var fileName in fileNames)
            {
                var file = GetIFormFile(fileName);
                if (file != null)
                {
                    files.Add(file);
                }
            }

            return files;
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream"; // Default type
            }
            return contentType;
        }



        //
        public async Task<List<(string filePath, string fileType)>> SaveUserFiles(ICollection<FileTuple> fileTuples)
        {
            var savedFiles = new List<(string, string)>();

            foreach (var fileTuple in fileTuples)
            {
                if (fileTuple.lstRealFile == null || !fileTuple.lstRealFile.Any())
                    continue;

                foreach (var file in fileTuple.lstRealFile)
                {
                    if (file.Length == 0)
                        continue;

                    var extension = Path.GetExtension(file.FileName).ToLower();

                    // Check if file is an image by extension
                    if (!_allowedImageExtensions.Contains(extension))
                    {
                        continue; // Skip non-image files
                    }

                    var folderPath = GetImageFolderPath(fileTuple.FileType, out var folderSubPath);
                    var fileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
                    var relativePath = Path.Combine("Uploaded", folderSubPath, fileName).Replace("\\", "/");

                    var absolutePath = Path.Combine(folderPath, fileName);



                    try
                    {
                        Directory.CreateDirectory(folderPath);
                        using (var stream = new FileStream(absolutePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        savedFiles.Add(("/" + relativePath, fileTuple.FileType)); // Leading slash added
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving image: {ex.Message}");
                    }
                }
            }

            return savedFiles;
        }

        private string GetImageFolderPath(string fileType ,out string folderSubPath)
        {
            

            folderSubPath= fileType.ToLower() switch
            {
                "personal image" => "Images/PersonalImages",
                "identity image" => "Images/IdentityImages",
                "property image" => "Images/PropertyImages",
                "property document" => "Document/PropertyDocument",
                "car document" => "Document/CarDocument",
                _ => "General"
            };

         return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploaded",folderSubPath);

        }

        // Method to Build Full URL
        public string GetFullFileUrl(string relativePath, HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return baseUrl + relativePath.Replace("\\", "/");
        }


        public bool IsImage(IFormFile file)
        {
            try
            {
                using (var image = Image.FromStream(file.OpenReadStream()))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
