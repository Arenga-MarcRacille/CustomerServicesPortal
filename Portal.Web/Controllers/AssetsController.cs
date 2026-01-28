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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _assetService.DeleteAssetAsync(id);
                TempData["SuccessMessage"] = "Asset deleted successfully.";
            }
            catch (Exception)
            {
                // This catches the Foreign Key Constraint Violation
                TempData["ErrorMessage"] = "Cannot delete this asset because it is currently assigned to existing Tickets. Please delete the tickets first.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit Form
        public async Task<IActionResult> Edit(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            var clients = await _assetService.GetAllClientsAsync();
            // Pre-select the current owner
            ViewBag.ClientId = new SelectList(clients, "ClientId", "FullName", asset.ClientId);

            return View(asset);
        }

        // POST: Update Asset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assets asset)
        {
            if (id != asset.AssetId)
            {
                return NotFound();
            }

            // Remove navigation property from validation
            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                try
                {
                    await _assetService.UpdateAssetAsync(asset);
                    TempData["SuccessMessage"] = "Asset details updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    // Basic concurrency or DB error handling
                    ModelState.AddModelError("", "Unable to save changes. Try again.");
                }
            }

            // If validation fails, reload dropdown
            var clients = await _assetService.GetAllClientsAsync();
            ViewBag.ClientId = new SelectList(clients, "ClientId", "FullName", asset.ClientId);

            return View(asset);
        }
    }
}