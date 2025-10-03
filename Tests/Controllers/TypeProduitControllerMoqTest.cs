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
    public class TypeProduitControllerMoqTests
    {
        private Mock<IDataRepository<TypeProduit>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private TypeProduitController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepo = new Mock<IDataRepository<TypeProduit>>();
            _mockMapper = new Mock<IMapper>();

            var mapperWrapper = new AutoMapperWrapperTypeProduit(_mockMapper.Object);
            _controller = new TypeProduitController(mapperWrapper, _mockRepo.Object);
        }

        private TypeProduit CreateTypeProduit(int id = 1, string nom = "Electromenager") =>
            new TypeProduit { IdTypeProduit = id, NomTypeProduit = nom };

        private TypeProduitDto CreateTypeProduitDto(int id = 1, string nom = "Electromenager") =>
            new TypeProduitDto { IdTypeProduit = id, NomTypeProduit = nom };

        [TestMethod]
        public async Task GetById_ShouldReturnTypeProduitDto_WhenExists()
        {
            var entity = CreateTypeProduit();
            var dto = CreateTypeProduitDto();

            _mockRepo.Setup(r => r.GetByIdAsync(entity.IdTypeProduit))
                     .ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<TypeProduitDto>(entity)).Returns(dto);

            var result = await _controller.Get(entity.IdTypeProduit);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(dto.NomTypeProduit, result.Value.NomTypeProduit);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((TypeProduit?)null);

            var result = await _controller.Get(999);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnListOfDtos()
        {
            var entities = new List<TypeProduit>
            {
                CreateTypeProduit(1),
                CreateTypeProduit(2, "Meuble")
            };
            var dtos = new List<TypeProduitDto>
            {
                CreateTypeProduitDto(1),
                CreateTypeProduitDto(2, "Meuble")
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new ActionResult<IEnumerable<TypeProduit>>(entities));
            _mockMapper.Setup(m => m.Map<IEnumerable<TypeProduitDto>>(entities))
                       .Returns(dtos);

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var value = okResult!.Value as IEnumerable<TypeProduitDto>;
            Assert.IsNotNull(value);
            Assert.AreEqual(2, value.Count());
            CollectionAssert.AreEqual(dtos.ToList(), value.ToList());
        }

        [TestMethod]
        public async Task Create_ShouldCallAddAsync_AndReturnCreatedDto()
        {
            var dto = CreateTypeProduitDto();
            var entity = CreateTypeProduit();

            _mockMapper.Setup(m => m.Map<TypeProduit>(dto)).Returns(entity);
            _mockMapper.Setup(m => m.Map<TypeProduitDto>(entity)).Returns(dto);

            var result = await _controller.Create(dto);

            _mockRepo.Verify(r => r.AddAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));

            var createdResult = result.Result as CreatedAtActionResult;
            var createdDto = createdResult!.Value as TypeProduitDto;
            Assert.IsNotNull(createdDto);
            Assert.AreEqual(dto.NomTypeProduit, createdDto!.NomTypeProduit);
        }

        [TestMethod]
        public async Task Update_ShouldCallUpdateAsync_WhenExists()
        {
            var dto = CreateTypeProduitDto();
            var entity = CreateTypeProduit();

            _mockRepo.Setup(r => r.GetByIdAsync(entity.IdTypeProduit))
                     .ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<TypeProduit>(dto)).Returns(entity);

            var result = await _controller.Update(entity.IdTypeProduit, dto);

            _mockRepo.Verify(r => r.UpdateAsync(entity, entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenDoesNotExist()
        {
            var dto = CreateTypeProduitDto(999, "Inexistant");

            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((TypeProduit?)null);

            var result = await _controller.Update(999, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ShouldCallDeleteAsync_WhenExists()
        {
            var entity = CreateTypeProduit();

            _mockRepo.Setup(r => r.GetByIdAsync(entity.IdTypeProduit))
                     .ReturnsAsync(entity);

            var result = await _controller.Delete(entity.IdTypeProduit);

            _mockRepo.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_WhenDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((TypeProduit?)null);

            var result = await _controller.Delete(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }

    // Wrapper AutoMapper pour les DTO
    public class AutoMapperWrapperTypeProduit : App.Mapper.IMapper<TypeProduit, TypeProduitDto>
    {
        private readonly IMapper _mapper;
        public AutoMapperWrapperTypeProduit(IMapper mapper) => _mapper = mapper;

        public TypeProduitDto ToDTO(TypeProduit entity) => _mapper.Map<TypeProduitDto>(entity);
        public TypeProduit ToEntity(TypeProduitDto dto) => _mapper.Map<TypeProduit>(dto);
        public IEnumerable<TypeProduitDto> ToDTOs(IEnumerable<TypeProduit> entities) => _mapper.Map<IEnumerable<TypeProduitDto>>(entities);
    }
}
