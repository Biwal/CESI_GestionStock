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
    public class ProviderController : ControllerBase
    {
        private readonly NegosudContext _context;

        public ProviderController(NegosudContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProviders(int pageSize = 10, int pageNumber = 1)
        {
            var response = new PagedResponse<Provider>();

            try
            {
                var query = _context.Providers.AsQueryable<Provider>();

                response.PageSize = pageSize;
                response.PageNumber = pageNumber;

                response.ItemsCount = await query.CountAsync();

                response.Model = await query.Paging(pageSize, pageNumber).ToListAsync();

                response.Message = string.Format("Page {0} of {1}, Total of providers: {2}.", pageNumber, response.PageCount, response.ItemsCount);
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support. : " + ex.Message;

            }

            return response.ToHttpResponse();
        }

        [HttpPost]
        public async Task<IActionResult> PostProductAsync([FromBody]Provider request)
        {
            var response = new SingleResponse<Provider>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                _context.Providers.Add(request);

                await _context.SaveChangesAsync();

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
