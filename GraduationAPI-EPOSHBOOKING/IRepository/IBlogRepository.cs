using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IBlogRepository
    {
        public ResponseMessage GetAllBlogs();
        public ResponseMessage GetBlogDetailById(int blogId);
        public ResponseMessage GetBlogsByAccountId(int accountId);
        public ResponseMessage CreateBlog(string title, string description, string location, string status, string imageData, int accountId);
        public ResponseMessage DeleteBlog(int blogId);

        //public ResponseMessage CreateComment(int blogId, int accountId, string description);
    }
}
