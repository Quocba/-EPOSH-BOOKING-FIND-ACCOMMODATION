using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly DBContext _dbContext;

        public BlogRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResponseMessage GetAllBlogs()
        {
            try
            {
                var blogs = _dbContext.blog
                    .Include(b => b.Comment)
                    .Include(b => b.BlogImage)
                    .ToList();

                if (blogs != null && blogs.Any())
                {
                    return new ResponseMessage { Success = true, Message = "Successfully", Data = blogs, StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    return new ResponseMessage { Success = false, Message = "No blogs found", StatusCode = (int)HttpStatusCode.NotFound };
                }
            }
            catch (Exception ex)
            {
                // Xử lý các trường hợp ngoại lệ nếu cần
                return new ResponseMessage { Success = false, Message = ex.Message, StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        public ResponseMessage GetBlogDetailById(int blogId)
        {
            try
            {
                var blog = _dbContext.blog
                    .Include(b => b.Comment)
                    .Include(b => b.BlogImage)
                    .FirstOrDefault(b => b.BlogID == blogId);

                if (blog != null)
                {
                    return new ResponseMessage { Success = true, Message = "Successfully retrieved blog details", Data = blog, StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    return new ResponseMessage { Success = false, Message = "Blog not found", StatusCode = (int)HttpStatusCode.NotFound };
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Success = false, Message = ex.Message, StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
    }
}
