using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using WebData.Data;
using WebData.Models;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _context = db;
            _environment = environment;
        }

        [HttpGet("GetByCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            try
            {
                var products = await _context.products
                    .Where(p => p.ProductCategoryId == categoryId)
                    .Include(p => p.ProductCategory)
                    .ToListAsync();

                if (products == null || !products.Any())
                {
                    return NotFound(new { Message = "Không tìm thấy sản phẩm cho danh mục này." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi trên server.", Error = ex.Message });
            }
        }


        // Lấy danh sách sản phẩm
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (_context.products == null)
            {
                return NotFound();
            }

            var products = await _context.products
                .Include(x => x.ProductCategory)
                .ToListAsync();
            return Ok(products);
        }

        // Lấy thông tin sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // Thêm sản phẩm mới cùng với hình ảnh
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Xử lý lưu ảnh vào thư mục wwwroot/Images
            string imageFileName = null;
            if (model.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, imageFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }

            // Tạo đối tượng Product mới
            var product = new Product
            {
                Title = model.Title,
                ProductCode = model.ProductCode,
                Description = model.Description,
                Detail = model.Detail,
                Image = imageFileName,
                Price = model.Price,
                PriceSale = model.PriceSale,
                Quantity = model.Quantity,
                IsHome = model.IsHome,
                ProductCategoryId = model.ProductCategoryId
            };

            _context.products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // Cập nhật sản phẩm cùng với hình ảnh
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.products.FindAsync(id);
            if (product == null)
                return NotFound();

            // Cập nhật thông tin sản phẩm
            product.Title = model.Title;
            product.ProductCode = model.ProductCode;
            product.Description = model.Description;
            product.Detail = model.Detail;
            product.Price = model.Price;
            product.PriceSale = model.PriceSale;
            product.Quantity = model.Quantity;
            product.IsHome = model.IsHome;
            product.ProductCategoryId = model.ProductCategoryId;

            // Xử lý cập nhật hình ảnh
            if (model.ImageFile != null)
            {
                // Xóa hình ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(product.Image))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, "Images", product.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Lưu hình ảnh mới
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Images");
                var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, imageFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                product.Image = imageFileName;
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Xóa sản phẩm và hình ảnh liên quan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.products.FindAsync(id);
            if (product == null)
                return NotFound();

            // Xóa hình ảnh nếu tồn tại
            if (!string.IsNullOrEmpty(product.Image))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "Images", product.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetImage/{imageName}")]
        public async Task<IActionResult> GetImageFile(string imageName)
        {
            var imagePath = Path.Combine(_environment.WebRootPath, "Images", imageName);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }

            var imageFileStream = System.IO.File.OpenRead(imagePath);
            var mimeType = GetMimeType(imageName);
            return new FileStreamResult(imageFileStream, mimeType);
        }
        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }


    }
}