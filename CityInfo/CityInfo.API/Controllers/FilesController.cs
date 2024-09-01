using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
            ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));

        }

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var pathFile = "Getting started with rest slides.pdf";
            if (!System.IO.File.Exists(pathFile))
            {
                return NotFound();
            } 

            if(!_fileExtensionContentTypeProvider.TryGetContentType(pathFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathFile);
            return File(bytes, contentType, Path.GetFileName(pathFile));
        }

        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            //Validating the input. We put a limit on the filesize to avoid large uploads attacks.
            //Only accept .pdf files (content-type).
            if(file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
            {
                return BadRequest("No file or an invalid one has been inputted.");
            }

            //We create the file path. Avoid using the file.FileName, as an atacker can provide a 
            //malicious one, including full paths o relative ones.

            var path = Path.Combine(Directory.GetCurrentDirectory(),
                $"uploaded_file_{Guid.NewGuid()}.pdf");

            //it's time to copy the bytes to a stream and save that stream as a file 
            //we initialize a new File Stream and call copy to async on our inputted file
            //passing through the stream as a parameter. That should store the file.
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok("Your file has been uploaded succesfully.");    
        }
    }
}
