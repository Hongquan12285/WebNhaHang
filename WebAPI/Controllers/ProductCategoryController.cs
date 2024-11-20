using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebData.Data;
using WebData.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryController(ApplicationDbContext db)
        {
            _context = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var prd = await _context.productCategories.ToListAsync();
            return Ok(prd);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prd = await _context.productCategories.FirstOrDefaultAsync(x => x.ID == id);
            return Ok(prd);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory productCategory)
        {
            await _context.productCategories.AddAsync(productCategory);
            await _context.SaveChangesAsync();
            return Ok(productCategory);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(ProductCategory productCategory)
        {
            _context.productCategories.Update(productCategory);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kh = await _context.productCategories.FirstOrDefaultAsync(x => x.ID == id);
            if (kh != null)
            {
                _context.productCategories.Remove(kh);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}
