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
    public class ClientOrderController : ControllerBase
    {
        private readonly NegosudContext _context;

        public ClientOrderController(NegosudContext context)
        {
            _context = context;
        }

        // GET: api/Products

        [HttpGet]
        public async Task<IActionResult> GetClientOrders(int? clientId, int pageSize = 10, int pageNumber = 1)
        {
            var response = new PagedResponse<ClientOrder>();

            try
            {
                IQueryable<ClientOrder> query = _context.ClientOrders.AsQueryable<ClientOrder>().Include(c => c.ClientOrderItems).ThenInclude(ci => ci.Product).Include(c => c.Client).OrderBy(c => c.Status);
                if (clientId != null) query = query.Where(c => c.ClientId == clientId);

                // Set paging values
                response.PageSize = pageSize;
                response.PageNumber = pageNumber;

                // Get the total rows
                response.ItemsCount = await query.CountAsync();

                // Get the specific page from database
                response.Model = await query.Paging(pageSize, pageNumber).ToListAsync();

                response.Message = string.Format("Page {0} of {1}, Total of products: {2}.", pageNumber, response.PageCount, response.ItemsCount);
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
        public async Task<IActionResult> PostClientOrderAsync([FromBody]ClientOrder request)
        {
            var response = new SingleResponse<ClientOrder>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                // Add entity to repository
                _context.ClientOrders.Add(request);

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

        [HttpPut]
        public async Task<IActionResult> PutClientOrder(int id, ClientOrder clientOrder)
        {
            var response = new SingleResponse<ClientOrder>();

            if (id != clientOrder.Id)
            {
                return BadRequest();
            }

            ClientOrder co = _context.ClientOrders.Where(po => po.Id == clientOrder.Id).AsNoTracking().Include(co => co.ClientOrderItems).SingleOrDefault();

            _context.Entry(co).CurrentValues.SetValues(clientOrder);
            _context.Entry(co).State = EntityState.Modified;

            if (co != null)
            {
                response.Model = clientOrder;

                foreach (ClientOrderItem clientOrderItem in co.ClientOrderItems)
                {
                    if (!clientOrder.ClientOrderItems.Any(c => c.Id == clientOrderItem.Id))
                        _context.ClientOrderItems.Remove(clientOrderItem);
                }

                foreach (ClientOrderItem clientOrderItem in clientOrder.ClientOrderItems)
                {
                    ClientOrderItem existingChild = co.ClientOrderItems
                        .Where(c => c.Id == clientOrderItem.Id)
                        .SingleOrDefault();

                    if (existingChild != null)
                    {
                        _context.Entry(existingChild).CurrentValues.SetValues(clientOrderItem);
                        _context.Entry(existingChild).State = EntityState.Modified;
                    }
                    else
                    {
                        ClientOrderItem newClientOrderItem = new ClientOrderItem
                        {
                            ProductId = clientOrderItem.ProductId,
                            ClientOrderId = clientOrderItem.ClientOrderId,
                            Quantity = clientOrderItem.Quantity
                        };

                        _context.ClientOrderItems.Add(newClientOrderItem);
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
                        response.ErrorMessage = "ClientOrder not found";
                    }
                    throw;
                }
            }

            return response.ToHttpResponse();
        }

        private bool ProviderOrderExists(int id)
        {
            return _context.ClientOrders.Count(p => p.Id == id) > 0;
        }
    }
}
