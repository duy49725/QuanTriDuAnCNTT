using QTDA.DataAccess.Data;
using QTDA.DataAccess.Repository.IRepository;
using QTDA.Models;
using QTDA.Models.ViewModels;
using QTDA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;
using System.Text.Json.Serialization;
using System.Text.Json;
using AutoMapper;

namespace bulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductImageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
		private readonly IMapper _mapper;
		public ProductImageController(IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork, ApplicationDbContext db, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _db = db;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<ProductImage> productImages = _unitOfWork.ProductImage.GetAll(includeProperties: "Product").ToList();
			return View(productImages);
        }

        public IActionResult Upsert(int? id)
        {
            ProductImageVM productImageVM = new ProductImageVM()
            {
                productList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString(), 
                }),
                productImage = new ProductImage()
            };
            if(id == null || id == 0)
            {
                return View(productImageVM);
            }
            else
            {
                productImageVM.productImage = _unitOfWork.ProductImage.Get(u => u.Id == id);
                return View(productImageVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductImageVM productImageVM, IFormFile? file)
        {
			if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productImageVM.productImage.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productImageVM.productImage.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productImageVM.productImage.ImageUrl = @"\images\product\" + fileName;
                }
                if (productImageVM.productImage.Id == 0)
                {
                    _unitOfWork.ProductImage.Add(productImageVM.productImage);
                }
                else
                {
                    _unitOfWork.ProductImage.Update(productImageVM.productImage);
                }
                _unitOfWork.Save();
                TempData["success"] = "ProductImage created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productImageVM.productList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString()
                });
                return View(productImageVM);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ProductImage> objProductImages = _db.ProductImages.Select(u => new ProductImage
            {
                Id = u.Id,
                ImageUrl = u.ImageUrl,
                ProductId = u.ProductId,
				productTitle = u.Product.Title
			}).ToList();
           

            return Json(new { data = objProductImages });

		}

		public IActionResult Delete(int id)
        {
            var productImageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == id);
            if(productImageToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productImageToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.ProductImage.Remove(productImageToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = false, message = "Delete Successful" });
        }
    }
}
