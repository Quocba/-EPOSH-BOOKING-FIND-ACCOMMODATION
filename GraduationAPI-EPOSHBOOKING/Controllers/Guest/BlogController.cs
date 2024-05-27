using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
{
    [Route("api/v1/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        public class BlogRequest
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public string Status { get; set; }
            public string ImageData { get; set; }
            public int AccountID { get; set; }
        }
        public class CommentRequest
        {
            public int BlogID { get; set; }
            public int AccountID { get; set; }
            public string Description { get; set; }
        }
        [HttpGet("get-all-blog")]
        public IActionResult GetAllBlogs()
        {
           var response = _blogRepository.GetAllBlogs();
           return StatusCode(response.StatusCode,response);
        }
        [HttpGet("get-blog-details")]
        public IActionResult GetBlogDetailById([FromQuery]int blogId)
        {
            var response = _blogRepository.GetBlogDetailById(blogId);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("get-blog-by-account")]
        public IActionResult GetBlogsByAccountId([FromQuery]int accountId)
        {
        var response = _blogRepository.GetBlogDetailById(accountId);
        return StatusCode(response.StatusCode, response);     
        }
        [HttpPost("create-blog")]
        public IActionResult CreateBlog([FromBody] BlogRequest blogRequest)
        {
            var response = _blogRepository.CreateBlog(blogRequest.Title, blogRequest.Description, blogRequest.Location, blogRequest.Status, blogRequest.ImageData, blogRequest.AccountID);
            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete("delete-blog/{blogId}")]
        public IActionResult DeleteBlog(int blogId)
        {
            var response = _blogRepository.DeleteBlog(blogId);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("comment-blog")]
        public IActionResult CommentBlog([FromBody] CommentRequest commentRequest)
        {
            var response = _blogRepository.CommentBlog(commentRequest.BlogID, commentRequest.AccountID, commentRequest.Description);
            return StatusCode(response.StatusCode, response);
        }
    }
}
