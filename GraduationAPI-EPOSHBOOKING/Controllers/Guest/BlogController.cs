using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
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

        [HttpGet("get-all-blog")]
        public IActionResult GetAllBlogs()
        {
           var response = _blogRepository.GetAllBlogs();
           return StatusCode(response.StatusCode,response);
        }
        [HttpGet("Get-Blog-Detail-By-Id/{blogId}")]
        public IActionResult GetBlogDetailById(int blogId)
        {
            var response = _blogRepository.GetBlogDetailById(blogId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
