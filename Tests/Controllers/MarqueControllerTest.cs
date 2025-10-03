using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Controllers;
using App.DTO;
using App.Mapping;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Controllers
{
    [TestClass]
    public class MarqueControllerTests
    {
        private AppDbContext _context;
        private MarqueController _controller;
        private IMapper _mapper;
        private List<Marque> _testMarques;
        private int _initialMarqueCount;

        [TestInitialize]
        public void Initialize()
        {
            _context = new AppDbContext();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            // Nombre de marques existantes
            _initialMarqueCount = _context.Marques.Count();

            // Marques de test
            _testMarques = new List<Marque>
            {
                new Marque { IdMarque = 1000, NomMarque = "Ikea" },
                new Marque { IdMarque = 1001, NomMarque = "Conforama" }
            };
            _context.Marques.AddRange(_testMarques);
            _context.SaveChanges();

            var mapperWrapper = new AutoMapperWrapperMarque(_mapper);
            var manager = new MarqueManager(_context);
            _controller = new MarqueController(mapperWrapper, manager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Marques.RemoveRange(_testMarques);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnAllMarques()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var marques = okResult!.Value as IEnumerable<MarqueDto>;

            Assert.IsNotNull(marques);
            Assert.AreEqual(_testMarques.Count, marques.Count() - _initialMarqueCount);
            Assert.IsTrue(marques.Any(m => m.NomMarque == "Ikea"));
            Assert.IsTrue(marques.Any(m => m.NomMarque == "Conforama"));
        }

        [TestMethod]
        public async Task GetById_ShouldReturnMarqueDto_WhenExists()
        {
            var marque = _testMarques[0];

            var result = await _controller.Get(marque.IdMarque);
            var dto = result.Value;

            Assert.IsNotNull(dto);
            Assert.AreEqual(marque.NomMarque, dto.NomMarque);
        }

        [TestMethod]
        public async Task Create_ShouldAddMarque()
        {
            var dto = new MarqueDto { NomMarque = "But" };

            var result = await _controller.Create(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var marqueInDb = _context.Marques.FirstOrDefault(m => m.NomMarque == "But");
            Assert.IsNotNull(marqueInDb);

            _context.Marques.Remove(marqueInDb);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task Update_ShouldModifyExistingMarque()
        {
            var marque = _testMarques[0];
            var dto = new MarqueDto { IdMarque = marque.IdMarque, NomMarque = "IkeaModifie" };

            var result = await _controller.Update(marque.IdMarque, dto);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var updated = _context.Marques.Find(marque.IdMarque);
            Assert.AreEqual("IkeaModifie", updated.NomMarque);
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveMarque()
        {
            var marque = _testMarques[0];

            var result = await _controller.Delete(marque.IdMarque);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var deleted = _context.Marques.Find(marque.IdMarque);
            Assert.IsNull(deleted);

            _testMarques.Remove(marque);
        }
    }

    public class AutoMapperWrapperMarque : App.Mapper.IMapper<Marque, MarqueDto>
    {
        private readonly IMapper _mapper;
        public AutoMapperWrapperMarque(IMapper mapper) => _mapper = mapper;
        public MarqueDto ToDTO(Marque entity) => _mapper.Map<MarqueDto>(entity);
        public Marque ToEntity(MarqueDto dto) => _mapper.Map<Marque>(dto);
        public IEnumerable<MarqueDto> ToDTOs(IEnumerable<Marque> entities) => _mapper.Map<IEnumerable<MarqueDto>>(entities);
    }
}
