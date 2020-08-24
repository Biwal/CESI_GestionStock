using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProviderOrderController : ControllerBase
    {
        private readonly NegosudContext _context;

        public ProviderOrderController(NegosudContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProviderOrders(int? providerId, bool lastProviderOrder = false, int pageSize = 10, int pageNumber = 1)
        {
            var response = new PagedResponse<ProviderOrder>();

            try
            {
                IQueryable<ProviderOrder> query = _context.ProviderOrders.AsQueryable<ProviderOrder>().Include(c => c.ProviderOrderItems).ThenInclude(c => c.Product).Include(c => c.Provider).OrderBy(c => c.Status);
                if (providerId != null) query = query.Where(c => c.ProviderId == providerId);
                if (lastProviderOrder) query = query.Where(po => po.Status == Models.Utils.OrderStatus.NONE).OrderByDescending(po => po.CreatedAt).Take(1);

                response.PageSize = pageSize;
                response.PageNumber = pageNumber;

                response.ItemsCount = await query.CountAsync();

                response.Model = await query.Paging(pageSize, pageNumber).ToListAsync();

                response.Message = string.Format("Page {0} of {1}, Total of providerorders: {2}.", pageNumber, response.PageCount, response.ItemsCount);
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.StackTrace;
                response.Message = ex.Message;
            }

            return response.ToHttpResponse();
        }

        [HttpPost]
        public async Task<IActionResult> PostProviderOrderAsync([FromBody]ProviderOrder request)
        {
            var response = new SingleResponse<ProviderOrder>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                _context.ProviderOrders.Add(request);

                await _context.SaveChangesAsync();

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

        [HttpPut]
        public async Task<IActionResult> PutProviderOrder(int id, ProviderOrder providerOrder)
        {
            var response = new SingleResponse<ProviderOrder>();

            if (id != providerOrder.Id)
            {
                return BadRequest();
            }
           
            ProviderOrder po = _context.ProviderOrders.Where(po => po.Id == providerOrder.Id).AsNoTracking().Include(po => po.ProviderOrderItems).SingleOrDefault();

            _context.Entry(po).CurrentValues.SetValues(providerOrder);
            _context.Entry(po).State = EntityState.Modified;

            if (po != null)
            {
                response.Model = providerOrder;

                foreach (ProviderOrderItem providerOrderItem in po.ProviderOrderItems)
                {
                    if (!providerOrder.ProviderOrderItems.Any(c => c.Id == providerOrderItem.Id))
                        _context.ProviderOrderItems.Remove(providerOrderItem);
                }

                foreach (ProviderOrderItem providerOrderItem in providerOrder.ProviderOrderItems)
                {
                    ProviderOrderItem existingChild = po.ProviderOrderItems
                        .Where(c => c.Id == providerOrderItem.Id)
                        .SingleOrDefault();
                    
                    if (existingChild != null)
                    {
                        _context.Entry(existingChild).CurrentValues.SetValues(providerOrderItem);
                        _context.Entry(existingChild).State = EntityState.Modified;
                    }
                    else
                    {
                        ProviderOrderItem newProviderOrderItem = new ProviderOrderItem
                        {
                            ProductId = providerOrderItem.ProductId,
                            ProviderOrderId = providerOrderItem.ProviderOrderId,
                            Quantity = providerOrderItem.Quantity
                        };

                        Console.WriteLine("New " + newProviderOrderItem.Quantity);

                        _context.ProviderOrderItems.Add(newProviderOrderItem);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProviderOrderExists(id))
                    {
                        response.DidError = true;
                        response.ErrorMessage = "Product not found";
                    }
                    throw;
                }
            }

            return response.ToHttpResponse();
        }

        private bool ProviderOrderExists(int id)
        {
            return _context.ProviderOrders.Count(p => p.Id == id) > 0;
        }
    }
}
