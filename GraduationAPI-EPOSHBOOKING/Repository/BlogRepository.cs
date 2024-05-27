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
        private readonly DBContext db;

        public BlogRepository(DBContext dbContext)
        {
            db = dbContext;
        }

        public ResponseMessage GetAllBlogs()
        {
            try
            {
                var blogs = db.blog
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
                return new ResponseMessage { Success = false, Message = ex.Message, StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        public ResponseMessage GetBlogDetailById(int blogId)
        {
            try
            {
                var blog = db.blog
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

        public ResponseMessage GetBlogsByAccountId(int accountId)
        {
            var getBlog = db.blog.Include(img => img.BlogImage).Include(account => account.Account)
                .Where(blog => blog.Account.AccountID == accountId);
            if (getBlog != null)
            {
                return new ResponseMessage { Success = true, Data = getBlog, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = getBlog, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound };
        }
        public ResponseMessage CreateBlog(string title, string description, string location, string status, string imageData, int accountId)
        {
            var account = db.accounts.FirstOrDefault(a => a.AccountID == accountId);
            if (account == null)
            {
                return new ResponseMessage { Success = false, Data = accountId, Message = "Account not found", StatusCode = (int)HttpStatusCode.NotFound };
            }
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(imageData))
            {
                return new ResponseMessage { Success = false, Data = null, Message = "Title, Description, Location, ImageData is required", StatusCode = (int)HttpStatusCode.BadRequest };
            }
            Blog blog = new Blog
            {
                Title = title,
                Description = description,
                Location = location,
                Status = "",
                ReasonReject = "",
                Account = account
            };

            db.blog.Add(blog);
            db.SaveChanges();
            BlogImage blogImage = new BlogImage
            {
                ImageData = System.Text.Encoding.UTF8.GetBytes(imageData),
                Blog = blog
            };
            db.blogImage.Add(blogImage);
            db.SaveChanges();
            var createdBlog = db.blog.Include(img => img.BlogImage).Include(a => a.Account).FirstOrDefault(b => b.BlogID == blog.BlogID);
            return new ResponseMessage { Success = true, Data = createdBlog, Message = "Blog created successfully", StatusCode = (int)HttpStatusCode.OK };
        }

        public ResponseMessage CommentBlog(int blogId, int accountId, string description)
        {
            var blog = db.blog.Include(b => b.Comment).FirstOrDefault(b => b.BlogID == blogId);
            if (blog == null)
            {
                return new ResponseMessage { Success = false, Data = blogId, Message = "Blog not found", StatusCode = (int)HttpStatusCode.NotFound };
            }

            var account = db.accounts.FirstOrDefault(a => a.AccountID == accountId);
            if (account == null)
            {
                return new ResponseMessage { Success = false, Data = accountId, Message = "Account not found", StatusCode = (int)HttpStatusCode.NotFound };
            }

            if (string.IsNullOrEmpty(description))
            {
                return new ResponseMessage { Success = false, Data = null, Message = "Description is required", StatusCode = (int)HttpStatusCode.BadRequest };
            }
            CommentBlog comment = new CommentBlog
            {
                BlogID = blogId,
                AccountID = accountId,
                Desciption = description,

                DateComment = DateTime.Now
            };
            db.blogComment.Add(comment);
            db.SaveChanges();

            var createdComment = db.blogComment
                                    .Include(c => c.Blog)
                                    .Include(c => c.Account)
                                    .FirstOrDefault(c => c.BlogID == blogId && c.AccountID == accountId && c.Desciption == description);

            return new ResponseMessage { Success = true, Data = comment, Message = "Comment created successfully", StatusCode = (int)HttpStatusCode.OK };
        }
        public ResponseMessage DeleteBlog(int blogId)
        {
            var blog = db.blog.Include(b => b.BlogImage).Include(b => b.Comment).FirstOrDefault(b => b.BlogID == blogId);
            if (blog == null)
            {
                return new ResponseMessage { Success = false, Data = blogId, Message = "Blog not found", StatusCode = (int)HttpStatusCode.NotFound };
            }

            foreach (var comment in blog.Comment)
            {
                db.blogComment.Remove(comment);
            }
            foreach (var image in blog.BlogImage)
            {
                db.blogImage.Remove(image);
            }
            db.blog.Remove(blog);
            db.SaveChanges();

            return new ResponseMessage { Success = true, Data = blogId, Message = "Blog deleted successfully", StatusCode = (int)HttpStatusCode.OK };
        }

    }
}
