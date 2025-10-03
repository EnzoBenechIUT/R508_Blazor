using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Controllers;
using App.DTO;
using App.Models;
using App.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.Controllers
{
    [TestClass]
    public class MarqueControllerMoqTests
    {
        private Mock<IDataRepository<Marque>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private MarqueController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepo = new Mock<IDataRepository<Marque>>();
            _mockMapper = new Mock<IMapper>();

            var mapperWrapper = new AutoMapperWrapperMarque(_mockMapper.Object);
            _controller = new MarqueController(mapperWrapper, _mockRepo.Object);
        }

        private Marque CreateMarque(int id = 1, string nom = "Ikea") =>
            new Marque { IdMarque = id, NomMarque = nom };

        private MarqueDto CreateMarqueDto(int id = 1, string nom = "Ikea") =>
            new MarqueDto { IdMarque = id, NomMarque = nom };

        [TestMethod]
        public async Task GetById_ShouldReturnMarqueDto_WhenExists()
        {
            var marque = CreateMarque();
            var dto = CreateMarqueDto();

            _mockRepo.Setup(r => r.GetByIdAsync(marque.IdMarque))
                     .ReturnsAsync(marque);
            _mockMapper.Setup(m => m.Map<MarqueDto>(marque)).Returns(dto);

            var result = await _controller.Get(marque.IdMarque);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(dto, result.Value);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Marque?)null);

            var result = await _controller.Get(999);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnList()
        {
            var marques = new List<Marque>
            {
                CreateMarque(1, "Ikea"),
                CreateMarque(2, "Conforama")
            };

            var dtos = new List<MarqueDto>
            {
                CreateMarqueDto(1, "Ikea"),
                CreateMarqueDto(2, "Conforama")
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(marques);
            _mockMapper.Setup(m => m.Map<IEnumerable<MarqueDto>>(It.IsAny<IEnumerable<Marque>>()))
                       .Returns(dtos);

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var value = okResult.Value as IEnumerable<MarqueDto>;
            Assert.IsNotNull(value);
            Assert.AreEqual(2, value.Count());
            CollectionAssert.AreEqual(dtos.ToList(), value.ToList());
        }

        [TestMethod]
        public async Task Create_ShouldCallAddAsync()
        {
            var dto = CreateMarqueDto();
            var marque = CreateMarque();

            _mockMapper.Setup(m => m.Map<Marque>(dto)).Returns(marque);
            _mockMapper.Setup(m => m.Map<MarqueDto>(marque)).Returns(dto);

            var result = await _controller.Create(dto);

            _mockRepo.Verify(r => r.AddAsync(marque), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task Update_ShouldCallUpdateAsync_WhenExists()
        {
            var dto = CreateMarqueDto();
            var marque = CreateMarque();

            _mockRepo.Setup(r => r.GetByIdAsync(marque.IdMarque))
                     .ReturnsAsync(marque);
            _mockMapper.Setup(m => m.Map<Marque>(dto)).Returns(marque);

            var result = await _controller.Update(marque.IdMarque, dto);

            _mockRepo.Verify(r => r.UpdateAsync(marque, marque), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenDoesNotExist()
        {
            var dto = CreateMarqueDto();

            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Marque?)null);

            var result = await _controller.Update(999, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ShouldCallDeleteAsync_WhenExists()
        {
            var marque = CreateMarque();

            _mockRepo.Setup(r => r.GetByIdAsync(marque.IdMarque))
                     .ReturnsAsync(marque);

            var result = await _controller.Delete(marque.IdMarque);

            _mockRepo.Verify(r => r.DeleteAsync(marque), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_WhenDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Marque?)null);

            var result = await _controller.Delete(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
