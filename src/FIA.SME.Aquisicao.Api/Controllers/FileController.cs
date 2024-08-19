using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/arquivo")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ISMEStorage _storage;

        public FileController(ISMEStorage storage)
        {
            this._storage = storage;
        }

        [HttpGet("{cooperative_id}/{fileName}")]
        public async Task<IActionResult> GetFile(Guid cooperative_id, string fileName)
        {
            string filePath = await this._storage.GetFilePath(cooperative_id, fileName);

            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf");
            }

            return NotFound();
        }
    }
}
