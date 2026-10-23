using Day2.DTOS;
using Day2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfAdvancedTask;

[ApiController]
[Route("api/products")]
public class ProductsController(AppDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        int pageNumber = 1, int pageSize = 10, string? search = null)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.StartsWith(search));

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Price, p.Category.Name, p.RowVersion))
            .ToListAsync();

        return new PagedResult<ProductDto>(items, pageNumber, pageSize, total);
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {

        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
            return NotFound();

        if (!await context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
            return BadRequest("Category not found.");


        context.Entry(product).Property(p => p.RowVersion).OriginalValue = dto.RowVersion;


        product.Name = dto.Name;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            var dbValues = await entry.GetDatabaseValuesAsync();

            if (dbValues is null)
                return NotFound("The product was deleted by another user.");


            return Conflict(new
            {
                message = "The product was modified by another user.",
                current = new
                {
                    Name = dbValues.GetValue<string>(nameof(Product.Name)),
                    Price = dbValues.GetValue<decimal>(nameof(Product.Price)),
                    CategoryId = dbValues.GetValue<int>(nameof(Product.CategoryId)),
                    RowVersion = dbValues.GetValue<byte[]>(nameof(Product.RowVersion))
                }
            });
        }

        return NoContent();
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
            return NotFound();

        product.IsDeleted = true;
        await context.SaveChangesAsync();
        return NoContent();
    }


    [HttpGet("deleted")]
    public async Task<ActionResult<List<ProductDto>>> GetDeleted() =>
        await context.Products
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(p => p.IsDeleted)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Category.Name, p.RowVersion))
            .ToListAsync();
}

