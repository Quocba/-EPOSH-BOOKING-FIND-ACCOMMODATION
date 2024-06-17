using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Customer
{
    [ApiController]
    [Route("api/v1/customer/blog")]
    public class CustomerBlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;

        public CustomerBlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        [HttpGet("get-blog-by-account")]
        public IActionResult GetBlogsByAccountId([FromQuery] int accountId)
        {
            var response = _blogRepository.GetBlogsByAccountId(accountId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("create-blog")]
        public IActionResult CreateBlog([FromForm] Blog blog, [FromForm] int accountID, [FromForm] List<IFormFile> image)
        {
            var response = _blogRepository.CreateBlog(blog, accountID, image);
            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete("delete-blog")]
        public IActionResult DeleteBlog([FromQuery] int blogId)
        {
            var response = _blogRepository.DeleteBlog(blogId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("comment-blog")]
        public IActionResult CommentBlog([FromForm] int blogId, [FromForm] int accountId, [FromForm] string description)
        {
            var response = _blogRepository.CommentBlog(blogId, accountId, description);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("filter-blog-with-status")]
        public IActionResult FilterBlogwithStatus([FromQuery] String status)
        {
            var response = _blogRepository.FilterBlogwithStatus(status);
            return StatusCode(response.StatusCode, response);
        }

    }
}
