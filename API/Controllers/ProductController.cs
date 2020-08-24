using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly NegosudContext _context;

        public ProductController(NegosudContext context)
        {
            _context = context;
        }

        // GET: api/Products

        [HttpGet]
        public async Task<IActionResult> GetProducts(int? providerId, bool toUpdate = false, int pageSize = 10, int pageNumber = 1)
        {
            var response = new PagedResponse<Product>();

            try
            {
                IQueryable<Product> query = _context.Products.AsQueryable<Product>().Include(p => p.Family).Include(p => p.Provider);
                if (providerId != null) query = query.Where(p => p.ProviderId == providerId);
                if (toUpdate) query = query.Where(p => p.Quantity <= p.MinStockAvailable);

                response.PageSize = pageSize;
                response.PageNumber = pageNumber;

                response.ItemsCount = await query.CountAsync();

                response.Model = await query.Paging(pageSize, pageNumber).ToListAsync();

                response.Message = string.Format("Page {0} of {1}, Total of families: {2}.", pageNumber, response.PageCount, response.ItemsCount);
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support. : " + ex.Message;

            }

            return response.ToHttpResponse();
        }

        [HttpPut]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            var response = new SingleResponse<Product>();

            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            response.Model = product;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    response.DidError = true;
                    response.ErrorMessage = "Product not found";
                }
                throw;
            }

            return response.ToHttpResponse();
        }

        [HttpPost]
        public async Task<IActionResult> PostProductAsync([FromBody]Product request)
        {
            var response = new SingleResponse<Product>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                // Add entity to repository
                _context.Products.Add(request);

                // Save entity in database
                await _context.SaveChangesAsync();

                // Set the entity to response model
                response.Model = request;
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.StackTrace;
                response.Message = ex.Message;
            }
            return response.ToHttpResponse();
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Count(p => p.Id == id) > 0;
        }
    }
   

}
