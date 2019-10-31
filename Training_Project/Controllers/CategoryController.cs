using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Training_Project.Data;
using Training_Project.Models;

namespace Training_Project.Controllers
{

    /// <summary>
    /// Category controller.
    /// </summary>
    [Route("api/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        private Training_ProjectContext _context;
        public CategoryController(Training_ProjectContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Selected category detail.
        /// </summary>
        /// <param name="id">Id of category.</param>
        /// <returns>Detail of selected category.</returns>
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(Categories), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult Detail(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id == 0)
                {
                    result.Message = "Category ID is not valid";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var categoryDetail = _context.Categories.Where(c => c.CategoryId == id && c.IsDeleted != true).FirstOrDefault();
                if (categoryDetail == null)
                {
                    result.Message = "Category does not exist";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                result.Body = categoryDetail;
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


        /// <summary>
        /// Insert category.
        /// </summary>
        /// <returns>
        /// Status of category interested.
        /// </returns>
        [HttpPost("insert")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult Insert()
        {
            var result = new Result
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
                var category = JsonConvert.DeserializeObject<Categories>(Request.Form["category"]);
                IFormFile img = null;
                if (Request.Form.Files.Count != 0)
                {
                    var image = Request.Form.Files;
                    img = image[0];
                }
                var categoryCheck = _context.Categories.Where(c => c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId && c.IsDeleted != true).FirstOrDefault();
                if (categoryCheck != null)
                {
                    result.Message = "Category of this name exists";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                category.CreatedDate = DateTime.Now;
                if (img != null)
                {
                    ImageExtension imageExtension = new ImageExtension();
                    category.CategoryImage = imageExtension.Image(img);
                }
                _context.Categories.Add(category);
                _context.SaveChanges();
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


        /// <summary>
        /// Update category.
        /// </summary>
        /// <returns>
        /// Status of category updated.
        /// </returns>
        [HttpPut("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult Update()
        {
            var result = new Result
            {
                Operation = Operation.Update,
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
                var category = JsonConvert.DeserializeObject<Categories>(Request.Form["category"]);
                IFormFile img = null;
                if (Request.Form.Files.Count != 0)
                {
                    var image = Request.Form.Files;
                    img = image[0];
                }
                var categoryDetail = _context.Categories.Where(c => c.CategoryId == category.CategoryId && c.IsDeleted != true).FirstOrDefault();
                if (categoryDetail == null)
                {
                    result.Message = "Category does not exist";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var duplicateName = _context.Categories.Where(c => c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId && c.IsDeleted != true).FirstOrDefault();
                if (duplicateName != null)
                {
                    result.Message = "Category of this name exists";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                categoryDetail.CategoryName = category.CategoryName;
                categoryDetail.CategoryDescription = category.CategoryDescription;
                categoryDetail.IsActive = category.IsActive;
                categoryDetail.CreatedBy = category.CreatedBy;
                categoryDetail.CreatedDate = category.CreatedDate;
                categoryDetail.ModifiedBy = category.ModifiedBy;
                categoryDetail.ModifiedDate = DateTime.Now;
                if (categoryDetail.CategoryImage != null)
                {
                    if (img == null)
                    {
                        categoryDetail.CategoryImage = null;
                    }
                    else
                    {
                        var imageExtension = new ImageExtension();
                        categoryDetail.CategoryImage = imageExtension.Image(img);
                    }
                }

                _context.SaveChanges();

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


        /// <summary>
        /// Category list.
        /// </summary>        
        /// <returns>
        /// Returns list of category.
        /// </returns>       
        [HttpGet("listing")]
        [ProducesResponseType(typeof(List<Categories>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult Listing()
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var categoryList = _context.Categories.Where(c => c.IsDeleted != true).OrderByDescending(c => c.CategoryId).ToList();
                if (categoryList.Count == 0)
                {
                    result.Message = "Category list is empty";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                result.Body = categoryList;
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            };
        }


        /// <summary>
        /// Deletes category.
        /// </summary>
        /// <param name="Id">Id of selected category.</param>
        /// <returns>
        /// Status message.
        /// </returns>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult Delete(int id)
        {
            var result = new Result()
            {
                Operation = Operation.Delete,
                Status = Status.Success
            };
            try
            {
                if (id == 0)
                {
                    result.Message = "Category ID is not valid";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var category = _context.Categories.Where(c => c.CategoryId == id && c.IsDeleted != true).FirstOrDefault();
                category.IsDeleted = true;
                _context.SaveChanges();
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