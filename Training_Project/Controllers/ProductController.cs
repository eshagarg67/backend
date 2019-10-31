using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Training_Project.Data;
using Training_Project.Models;

namespace Training_Project.Controllers
{

    /// <summary>
    /// Product controller.
    /// </summary>
    [Route("api/product")]
    [ApiController]
    public class ProductController : Controller
    {
        private Training_ProjectContext _context;
        public ProductController(Training_ProjectContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the details of product.
        /// </summary>
        /// <param name="id">Id of the product.</param>
        /// <returns>
        /// Details of the selected product.
        /// </returns>
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(Products), StatusCodes.Status206PartialContent)]
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
                    result.Message = "Product ID is not valid";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
               
                var productDetail = _context.Products.Where(p => p.ProductId == id && p.IsDeleted != true).FirstOrDefault();
                ProductDetail detail = new ProductDetail(productDetail);

                if (detail == null)
                {
                    result.Message = "Product does not exist";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                else
                {
                    detail.categoryName = _context.Categories.Where(x => x.CategoryId == productDetail.CategoryId).FirstOrDefault().CategoryName;
                }


                result.Body = detail;
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
        /// Inserts product.
        /// </summary>
        /// <param name="product">Object of ProductModel.</param>
        /// <returns>
        /// Status of product added.
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
                var product = JsonConvert.DeserializeObject<Products>(Request.Form["product"]);
                IFormFile img = null;
                if (Request.Form.Files.Count != 0)
                {
                    var image = Request.Form.Files;
                    img = image[0];
                }
                var pdtNameCheck = _context.Products.Where(p => p.ProductName == product.ProductName && p.ProductId != product.ProductId && p.IsDeleted != true).FirstOrDefault();
                if (pdtNameCheck != null)
                {
                    result.Message = "Product of this name exists already";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var pdtModelCheck = _context.Products.Where(p => p.ModelNumber == product.ModelNumber && p.ProductId != product.ProductId && p.IsDeleted != true).FirstOrDefault();
                if (pdtModelCheck != null)
                {
                    result.Message = "Product with this model number exists";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                product.CreatedDate = DateTime.Now;
                product.IsActive = true;
                if (img != null)
                {
                    ImageExtension imageExtension = new ImageExtension();
                    product.ProductImage = imageExtension.Image(img);
                }
                _context.Products.Add(product);
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
        /// Updates product details.
        /// </summary>
        /// <param name="product">Object of product model.</param>
        /// <returns>
        /// Status of product update.
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
                var product = JsonConvert.DeserializeObject<Products>(Request.Form["product"]);
                IFormFile img = null;
                if (Request.Form.Files.Count != 0)
                {
                    var image = Request.Form.Files;
                    img = image[0];
                }
                var productDetail = _context.Products.Where(p => p.ProductId == product.ProductId && p.IsDeleted != true).FirstOrDefault();
                if (productDetail == null)
                {
                    result.Message = "Product does not exist";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var pdtNameCheck = _context.Products.Where(p => p.ProductName == product.ProductName && p.ProductId != product.ProductId && p.IsDeleted != true).FirstOrDefault();
                if (pdtNameCheck != null)
                {
                    result.Message = "Product of this name exists";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var pdtModelCheck = _context.Products.Where(p => p.ModelNumber == product.ModelNumber && p.ProductId != product.ProductId && p.IsDeleted != true).FirstOrDefault();
                if (pdtModelCheck != null)
                {
                    result.Message = "Product with this model number exists";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                productDetail.ProductName = product.ProductName;
                productDetail.ProductDescription = product.ProductDescription;
                productDetail.IsActive = product.IsActive;
                productDetail.CategoryId = product.CategoryId;
                productDetail.IsDiscounted = product.IsDiscounted;
                productDetail.DiscountPercent = product.DiscountPercent;
                productDetail.ModelNumber = product.ModelNumber;
                productDetail.Price = product.Price;
                productDetail.QuantityInStock = product.QuantityInStock;
                productDetail.CreatedBy = product.CreatedBy;
                productDetail.CreatedDate = product.CreatedDate;
                productDetail.ModifiedBy = product.ModifiedBy;
                productDetail.ModifiedDate = DateTime.Now;
                if (productDetail.ProductImage != null)
                {
                    if (img == null)
                    {
                        productDetail.ProductImage = null;
                    }
                    else
                    {
                        var imageExtension = new ImageExtension();
                        productDetail.ProductImage = imageExtension.Image(img);
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
        /// List of products.
        /// </summary>
        /// <param name="dataHelper">DataHelper object of paging and sorting list.</param>
        /// <returns>
        /// Paged and sorted list of prodcuts.
        /// </returns>
        [HttpGet("listing")]
        [ProducesResponseType(typeof(List<Products>), StatusCodes.Status206PartialContent)]
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
                var productList = _context.Products.Where(p => p.IsDeleted != true).OrderByDescending(p => p.ProductId).ToList();
                if (productList.Count == 0)
                {
                    result.Message = "Product list is empty";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                result.Body = productList;
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
        /// Deletes the product.
        /// </summary>
        /// <param name="ID">Id of selected product.</param>
        /// <returns>
        /// Status code with success message.
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
                    result.Message = "Product ID is not valid";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var category = _context.Products.Where(p => p.ProductId == id && p.IsDeleted != true).FirstOrDefault();
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


        /// <summary>
        /// Product list.
        /// </summary>
        /// <param name="id">Id of selected category.</param>
        /// <returns>
        /// Returns list of products for selected category.
        /// </returns>
        [HttpGet("productlistbycategory/{id}")]
        [ProducesResponseType(typeof(List<Products>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult ProductListByCategory(int id)
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
                var category = _context.Categories.Where(c => c.CategoryId == id && c.IsDeleted != true).FirstOrDefault();
                if (category == null)
                {
                    result.Message = "Category does not exist";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                ProductListInfoByCategory list = new ProductListInfoByCategory
                {
                    categoryId = id,
                    categoryName = category.CategoryName
                };
                var productList = _context.Products.Where(p => p.CategoryId == id && p.IsDeleted != true).ToList();
                list.products = productList;
                result.Body = list;
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