using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
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

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <returns></returns>
        [HttpPost("signup")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult CreateUser()
        {
            var result = new Result()
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                IFormFile img = null;
                var userModel = JsonConvert.DeserializeObject<Users>(Request.Form["model"]);
                var image = Request.Form.Files;
                foreach (var i in image)
                {
                    img = image[0];
                }
                if (userModel != null)
                {
                    if (img == null)
                    {
                        result.Message = "User image does not exist";
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        return result;
                    }
                    var userDetailCheck = _context.Users.Where(u => (u.Username == userModel.Username) || (u.EmailId == userModel.EmailId)).FirstOrDefault();
                    if (userDetailCheck != null)
                    {
                        if (userDetailCheck.Username == userModel.Username)
                        {
                            result.Message = "Username is already taken";
                            result.Status = Status.Fail;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return result;
                        }
                        else
                        {
                            result.Message = "Email ID is already registered";
                            result.Status = Status.Fail;
                            result.StatusCode = HttpStatusCode.BadRequest;
                            return result;
                        }
                    }
                    ImageExtension imageExtension = new ImageExtension();
                    userModel.UserImage = imageExtension.Image(img);
                    _context.Users.Add(userModel);
                    _context.SaveChangesAsync();

                    return result;
                }
                result.Message = "Invalid user details entered";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
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