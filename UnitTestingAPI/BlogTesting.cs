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
        private BlogController BlogController;
        private Mock<IBlogRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IBlogRepository>();
            BlogController = new BlogController(_mockRepository.Object);
        }
        [Test]
        public void GetAllBlogs()
        {
            // Arrange
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.GetAllBlogs())
                .Returns(new ResponseMessage { Success = true, Data = blogsList, StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = BlogController.GetAllBlogs() as ObjectResult;
            // Assert
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetBlogbyID()
        {
            // Arrange
            Blog blog = new Blog { BlogID = 9, Title = "Blog 9", Description = "Description 9", Location = "Location 9" };

            _mockRepository.Setup(repo => repo.GetBlogDetailById(9))
                .Returns(new ResponseMessage { Success = true, Data = blog, StatusCode = (int)HttpStatusCode.OK });

            // Act
            var result = BlogController.GetBlogDetailById(9) as ObjectResult;
            // Assert
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void GetBlogNullID()
        {
            // Arrange
            Blog blog = null;

            _mockRepository.Setup(repo => repo.GetBlogDetailById(9))
                .Returns(new ResponseMessage { Success = false, Data = blog, StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = BlogController.GetBlogDetailById(9) as ObjectResult;
            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetNullBlog()
        {
            // Arrange
            List<Blog> blogsList = new List<Blog>();

            _mockRepository.Setup(repo => repo.GetAllBlogs())
                .Returns(new ResponseMessage { Success = false, Data = blogsList, StatusCode = (int)HttpStatusCode.NotFound });

            // Act
            var result = BlogController.GetAllBlogs() as ObjectResult;
            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        [Test]
        public void GetException()
        {
            // Arrange
            List<Blog> blogsList = new List<Blog>
            {
                new Blog { BlogID = 1, Title = "Blog 1", Description = "Description 1", Location = "Location 1"},
                new Blog { BlogID = 2, Title = "Blog 2", Description = "Description 2", Location = "Location 2"}
            };

            _mockRepository.Setup(repo => repo.GetAllBlogs())
                .Returns(new ResponseMessage { Success = false, Data = blogsList, StatusCode = (int)HttpStatusCode.InternalServerError });

            // Act
            var result = BlogController.GetAllBlogs() as ObjectResult;
            // Assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
