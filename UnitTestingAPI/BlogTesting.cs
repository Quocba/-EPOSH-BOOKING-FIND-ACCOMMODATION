using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Customer;
using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace UnitTestingAPI
{
    [TestFixture]
    public class BlogTesting
    {
        private GeneralBlogController BlogController;
        private CustomerBlogController CustomerBlogController;
        private GeneralBlogController GeneralBlogController;
        private AdminBlogController AdminBlogController;
        private Mock<IBlogRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IBlogRepository>();
            BlogController = new GeneralBlogController(_mockRepository.Object);
            CustomerBlogController = new CustomerBlogController(_mockRepository.Object);
            GeneralBlogController = new GeneralBlogController(_mockRepository.Object);
            AdminBlogController = new AdminBlogController(_mockRepository.Object);
        }
        [Test]
        public void GetAllBlogs()
        {
            
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.GetAllBlogs())
                .Returns(new ResponseMessage { Success = true, Data = blogsList, StatusCode = (int)HttpStatusCode.OK });

            
            var result = BlogController.GetAllBlogs() as ObjectResult;
            
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetBlogbyID()
        {
            
            Blog blog = new Blog { BlogID = 9, Title = "Blog 9", Description = "Description 9", Location = "Location 9" };

            _mockRepository.Setup(repo => repo.GetBlogDetailById(9))
                .Returns(new ResponseMessage { Success = true, Data = blog, StatusCode = (int)HttpStatusCode.OK });

            
            var result = BlogController.GetBlogDetailById(9) as ObjectResult;
            
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetBlogNullID()
        {
            
            Blog blog = null;

            _mockRepository.Setup(repo => repo.GetBlogDetailById(9))
                .Returns(new ResponseMessage { Success = false, Data = blog, StatusCode = (int)HttpStatusCode.NotFound });

            
            var result = BlogController.GetBlogDetailById(9) as ObjectResult;
            
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetNullBlog()
        {
            
            List<Blog> blogsList = new List<Blog>();

            _mockRepository.Setup(repo => repo.GetAllBlogs())
                .Returns(new ResponseMessage { Success = false, Data = blogsList, StatusCode = (int)HttpStatusCode.NotFound });

            
            var result = BlogController.GetAllBlogs() as ObjectResult;
            
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetException()
        {
            
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.GetAllBlogs())
                .Returns(new ResponseMessage { Success = false, Data = blogsList, StatusCode = (int)HttpStatusCode.InternalServerError });

            
            var result = BlogController.GetAllBlogs() as ObjectResult;
            
            Assert.AreEqual(500, result.StatusCode);
        }
        [Test]
        public void CreateBlog_Success()
        {
            Blog blog = new Blog { BlogID = 9, Title = "Blog 9", Description = "Description 9", Location = "Location 9" };

            _mockRepository.Setup(repo => repo.CreateBlog(blog, 9, null))
                .Returns(new ResponseMessage { Success = true, Data = blog, StatusCode = (int)HttpStatusCode.OK });


            var result = CustomerBlogController.CreateBlog(blog, 9, null) as ObjectResult;

            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void CreateBlog_Fail()
        {
            Blog blog = new Blog { BlogID = 9, Title = "Blog 9", Description = "Description 9", Location = "Location 9" };

            _mockRepository.Setup(repo => repo.CreateBlog(blog, 9, null))
                .Returns(new ResponseMessage { Success = false, Data = blog, StatusCode = (int)HttpStatusCode.InternalServerError });
        }
        [Test]
        public void DeleteBlog_Success()
        {
            _mockRepository.Setup(repo => repo.DeleteBlog(9))
                .Returns(new ResponseMessage { Success = true, StatusCode = (int)HttpStatusCode.OK });

            var result = CustomerBlogController.DeleteBlog(9) as ObjectResult;

            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void DeleteBlog_Fail()
        {
            _mockRepository.Setup(repo => repo.DeleteBlog(9))
                .Returns(new ResponseMessage { Success = false, StatusCode = (int)HttpStatusCode.InternalServerError });
        }
        [Test]
        public void CommentBlog_Success()
        {
            _mockRepository.Setup(repo => repo.CommentBlog(9, 9, "Comment"))
                .Returns(new ResponseMessage { Success = true, StatusCode = (int)HttpStatusCode.OK });

            var result = CustomerBlogController.CommentBlog(9, 9, "Comment") as ObjectResult;

            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void CommentBlog_Fail()
        {
            _mockRepository.Setup(repo => repo.CommentBlog(9, 9, "Comment"))
                .Returns(new ResponseMessage { Success = false, StatusCode = (int)HttpStatusCode.InternalServerError });
        }
        [Test]
        public void FilterBlogwithStatus_Success()
        {
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.FilterBlogwithStatus("Status"))
                .Returns(new ResponseMessage { Success = true, Data = blogsList, StatusCode = (int)HttpStatusCode.OK });

            var result = CustomerBlogController.FilterBlogwithStatus("Status") as ObjectResult;

            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void FilterBlogwithStatus_Fail()
        {
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.FilterBlogwithStatus("Status"))
                .Returns(new ResponseMessage { Success = false, Data = blogsList, StatusCode = (int)HttpStatusCode.InternalServerError });
        }
        [Test]
        public void GetBlogsByAccountId_Success()
        {
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.GetBlogsByAccountId(9))
                .Returns(new ResponseMessage { Success = true, Data = blogsList, StatusCode = (int)HttpStatusCode.OK });

            var result = CustomerBlogController.GetBlogsByAccountId(9) as ObjectResult;

            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetBlogsByAccountId_Fail()
        {
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.GetBlogsByAccountId(9))
                .Returns(new ResponseMessage { Success = false, Data = blogsList, StatusCode = (int)HttpStatusCode.InternalServerError });
        }
        [Test]
        public void ConfirmBlog_Success()
        {
            _mockRepository.Setup(repo => repo.ConfirmBlog(9))
                .Returns(new ResponseMessage { Success = true, StatusCode = (int)HttpStatusCode.OK });

            var result = AdminBlogController.ConfirmBlog(9) as ObjectResult; 

            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void ConfirmBlog_Fail()
        {
            _mockRepository.Setup(repo => repo.ConfirmBlog(9))
                .Returns(new ResponseMessage { Success = false, StatusCode = (int)HttpStatusCode.InternalServerError });
        }
        [Test]
        public void RejectBlog_Success()
        {
            _mockRepository.Setup(repo => repo.RejectBlog(9, "Reason"))
                .Returns(new ResponseMessage { Success = true, StatusCode = (int)HttpStatusCode.OK });

            var result = AdminBlogController.RejectBlog(9, "Reason") as ObjectResult;

            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void RejectBlog_Fail()
        {
            _mockRepository.Setup(repo => repo.RejectBlog(9, "Reason"))
                .Returns(new ResponseMessage { Success = false, StatusCode = (int)HttpStatusCode.InternalServerError });
        }
    }
}
