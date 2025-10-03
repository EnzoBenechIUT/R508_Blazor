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
    public class ProductControllerMoqTests
    {
        private Mock<IDataRepository<Produit>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private ProductController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepo = new Mock<IDataRepository<Produit>>();
            _mockMapper = new Mock<IMapper>();

            var mapperWrapper = new AutoMapperWrapper(_mockMapper.Object);

            _controller = new ProductController(mapperWrapper, _mockRepo.Object);
        }

        private Produit CreateProduit(int id = 1, string nom = "Chaise") =>
            new Produit { IdProduit = id, NomProduit = nom, Description = "Desc", StockReel = 5 };

        private ProduitDto CreateProduitDto(int id = 1, string nom = "Chaise") =>
            new ProduitDto { Id = id, Nom = nom, Description = "Desc", StockReel = 5 };

        [TestMethod]
        public async Task GetById_ShouldReturnProduitDto_WhenExists()
        {
            var produit = CreateProduit();
            var dto = CreateProduitDto();

            _mockRepo.Setup(r => r.GetByIdAsync(produit.IdProduit))
                     .ReturnsAsync(produit);
            _mockMapper.Setup(m => m.Map<ProduitDto>(produit)).Returns(dto);

            var result = await _controller.Get(produit.IdProduit);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(dto, result.Value);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Produit?)null);

            var result = await _controller.Get(999);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnList()
        {
            var produits = new List<Produit> { CreateProduit(1), CreateProduit(2, "Table") };
            var dtos = new List<ProduitDto> { CreateProduitDto(1), CreateProduitDto(2, "Table") };

            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new ActionResult<IEnumerable<Produit>>(produits));

            _mockMapper.Setup(m => m.Map<IEnumerable<ProduitDto>>(It.IsAny<IEnumerable<Produit>>()))
                       .Returns(dtos);

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var value = okResult.Value as IEnumerable<ProduitDto>;

            Assert.IsNotNull(value);
            Assert.AreEqual(2, value.Count());
            CollectionAssert.AreEqual(dtos.ToList(), value.ToList());
        }


        [TestMethod]
        public async Task Create_ShouldCallAddAsync()
        {
            var dto = CreateProduitDto();
            var produit = CreateProduit();

            _mockMapper.Setup(m => m.Map<Produit>(dto)).Returns(produit);
            _mockMapper.Setup(m => m.Map<ProduitDto>(produit)).Returns(dto);

            var result = await _controller.Create(dto);

            _mockRepo.Verify(r => r.AddAsync(produit), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task Update_ShouldCallUpdateAsync_WhenExists()
        {
            var dto = CreateProduitDto();
            var produit = CreateProduit();

            _mockRepo.Setup(r => r.GetByIdAsync(produit.IdProduit))
                     .ReturnsAsync(produit);
            _mockMapper.Setup(m => m.Map<Produit>(dto)).Returns(produit);

            var result = await _controller.Update(produit.IdProduit, dto);

            _mockRepo.Verify(r => r.UpdateAsync(produit, produit), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenDoesNotExist()
        {
            var dto = CreateProduitDto();

            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Produit?)null);

            var result = await _controller.Update(999, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ShouldCallDeleteAsync_WhenExists()
        {
            var produit = CreateProduit();

            _mockRepo.Setup(r => r.GetByIdAsync(produit.IdProduit))
                     .ReturnsAsync(produit);

            var result = await _controller.Delete(produit.IdProduit);

            _mockRepo.Verify(r => r.DeleteAsync(produit), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_WhenDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Produit?)null);

            var result = await _controller.Delete(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
