using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal.Data;
using Portal.Services;
using Portal.Services.Interfaces;

namespace Portal.Web.Controllers
{
    [Authorize(Roles = "Admin")] // <--- SECURITY GATE
    public class AssetsController : Controller
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        // GET: List of all Assets
        public async Task<IActionResult> Index()
        {
            var assets = await _assetService.GetAllAssetsAsync();
            return View(assets);
        }

        // GET: Create Form
        public async Task<IActionResult> Create()
        {
            var clients = await _assetService.GetAllClientsAsync();

            // Dropdown: Value = ClientId, Text = FullName
            ViewBag.ClientId = new SelectList(clients, "ClientId", "FullName");

            return View();
        }

        // POST: Save Asset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assets asset)
        {
            // Remove the navigation property from validation
            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                await _assetService.CreateAssetAsync(asset);
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown if validation fails
            var clients = await _assetService.GetAllClientsAsync();
            ViewBag.ClientId = new SelectList(clients, "ClientId", "FullName", asset.ClientId);

            return View(asset);
        }
    }
}