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

    /// <summary>
    /// User controller.
    /// </summary>
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private Training_ProjectContext _context;
        public UserController(Training_ProjectContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="loginModel">username and password of user.</param>
        /// <returns>
        /// Token string for correct details.
        /// </returns>
        [HttpGet("login")]
        [ProducesResponseType(typeof(Users), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult LoginUser([FromQuery] LoginModel loginModel)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var userDetails = _context.Users.Where(u => u.EmailId == loginModel.Username && u.Password == loginModel.Password).FirstOrDefault();
                if (userDetails != null)
                {
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = userDetails;
                    return result;
                }
                else
                {
                    result.Message = "User details do not match";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                } 
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

    }
}