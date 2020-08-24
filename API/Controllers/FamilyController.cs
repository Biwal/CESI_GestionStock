using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyController : ControllerBase
    {
        private readonly NegosudContext _context;

        public FamilyController(NegosudContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFamilies(int pageSize = 100, int pageNumber = 1)
        {
            var response = new PagedResponse<Family>();

            try
            {
                // Get the "proposed" query from repository
                var query = _context.Families.AsQueryable<Family>();

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
                response.ErrorMessage = "There was an internal error, please contact to technical support. : " + ex.Message;

            }

            return response.ToHttpResponse();
        }
   
        [HttpPost]
        public async Task<IActionResult> PostFamilyAsync([FromBody]Family request)
        {
            var response = new SingleResponse<Family>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                // Add entity to repository
                _context.Families.Add(request);

                // Save entity in database
                await _context.SaveChangesAsync();

                // Set the entity to response model
                response.Model = request;
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support. " + ex.Message;
            }
            return response.ToHttpResponse();
        }
    }
}