using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/blog")]
    public class AdminBlogController : Controller
    {

        private readonly IBlogRepository _blogRepository;   
        public AdminBlogController(IBlogRepository blogRepository) { 
        
            this._blogRepository = blogRepository;
        }

        [HttpPut("confirm-blog")]
        public IActionResult ConfirmBlog([FromQuery] int blogId)
        {
            var response = _blogRepository.ConfirmBlog(blogId);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPut("reject-blog")]
        public IActionResult RejectBlog([FromForm] int blogId, [FromForm] string reasonReject)
        {
            var response = _blogRepository.RejectBlog(blogId, reasonReject);
            return StatusCode(response.StatusCode, response);
        }
    }
}
