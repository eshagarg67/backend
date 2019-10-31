using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Training_Project.Data;
using Training_Project.Models;
using Training_Project.Models.LocalModels;

namespace Training_Project.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : Controller
    {
        private Training_ProjectContext _context;
        public DashboardController(Training_ProjectContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns statisitics of categories and products
        /// </summary>
        /// <returns>
        /// Returns count for active categories and products
        /// </returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(DashboardModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult Statistics()
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var activeCategory = _context.Categories.Where(c => c.IsDeleted != true).Count();
                var activeProduct = _context.Products.Where(p => p.IsDeleted != true).Count();
                var dashboardModel = new DashboardModel()
                {
                    Categorycount = activeCategory,
                    ProductCount = activeProduct
                };
                result.Body = dashboardModel;
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }
    }
}