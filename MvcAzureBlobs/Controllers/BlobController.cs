using Microsoft.AspNetCore.Mvc;
using MvcAzureBlobs.Models;
using MvcAzureBlobs.Services;

namespace MvcAzureBlobs.Controllers
{
    public class BlobController : Controller
    {
        private ServiceStorageBlobs service;
        public BlobController(ServiceStorageBlobs service)
        {
            this.service = service;
        }

        public IActionResult SubirImagen()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubirImagen(IFormFile file)
        {
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadBlobAsync("contenedorpublico", file.FileName, stream);
            }

            return RedirectToAction("Imagenes");
        }



        public async Task<IActionResult> Imagenes()
        {
            List<ModeloAzureBlobs> modelpublico = await this.service.GetBlobsAsync("contenedorpublico");
            ViewData["PUBLICO"] = modelpublico;
            List<ModeloAzureBlobs> modelprivado = await this.service.GetBlobsAsync("contenedorprivado");
            ViewData["PRIVADO"] = modelprivado;
            return View();
        }

        public async Task<IActionResult> MoverPublico(string nombre)
        {
            await this.service.MoveBlobs("contenedorprivado", "contenedorpublico", nombre);
            return RedirectToAction("Imagenes");
        }

        public async Task<IActionResult> MoverPrivado(string nombre)
        {
            await this.service.MoveBlobs("contenedorpublico", "contenedorprivado", nombre);
            return RedirectToAction("Imagenes");
        }

        
    }
}
