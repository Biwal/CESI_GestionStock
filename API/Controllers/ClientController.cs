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
    public class ClientController : ControllerBase
    {
        private readonly NegosudContext _context;

        public ClientController(NegosudContext context)
        {
            _context = context;
        }

        // GET: api/Products

        [HttpGet]
        public async Task<IActionResult> GetClients(int pageSize = 10, int pageNumber = 1)
        {
            var response = new PagedResponse<Client>();

            try
            {
                var query = _context.Clients.AsQueryable<Client>().Include(c => c.ClientOrders);

                response.PageSize = pageSize;
                response.PageNumber = pageNumber;
                response.ItemsCount = await query.CountAsync();
                response.Model = await query.Paging(pageSize, pageNumber).ToListAsync();
                response.Message = string.Format("Page {0} of {1}, Total of clients: {2}.", pageNumber, response.PageCount, response.ItemsCount);
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support. : " + ex.Message;

            }

            return response.ToHttpResponse();
        }

        [HttpPost]
        public async Task<IActionResult> PostClientAsync([FromBody]Client request)
        {
            var response = new SingleResponse<Client>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                // Add entity to repository
                _context.Clients.Add(request);

                // Save entity in database
                await _context.SaveChangesAsync();

                // Set the entity to response model
                response.Model = request;
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support.";
            }
            return response.ToHttpResponse();
        }
    }
   

}
