using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IBlogRepository
    {
        public ResponseMessage GetAllBlogs();
        public ResponseMessage GetBlogDetailById(int blogId);
        ResponseMessage GetBlogsByAccountId(int accountId);
    }
}
