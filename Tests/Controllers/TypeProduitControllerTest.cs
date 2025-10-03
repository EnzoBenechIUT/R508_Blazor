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
    public class TypeProduitControllerTests
    {
        private AppDbContext _context;
        private TypeProduitController _controller;
        private IMapper _mapper;
        private List<TypeProduitDto> _testTypes;
        private int _initialTypeCount;

        [TestInitialize]
        public void Initialize()
        {
            _context = new AppDbContext();

            // AutoMapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            // Nombre de types existants
            _initialTypeCount = _context.TypeProduits.Count();

            // Types de test (IDs élevés pour éviter les collisions DB)
            _testTypes = new List<TypeProduitDto>
            {
                new TypeProduitDto { IdTypeProduit = 10000, NomTypeProduit = "Electromenager" },
                new TypeProduitDto { IdTypeProduit = 10001, NomTypeProduit = "Meuble" }
            };

            // Ajout en DB via AutoMapper
            var entities = _testTypes.Select(dto => _mapper.Map<TypeProduit>(dto)).ToList();
            _context.TypeProduits.AddRange(entities);
            _context.SaveChanges();

            // Controller
            var mapperWrapper = new AutoMapperWrapperTypeProduit(_mapper);
            var manager = new TypeProduitManager(_context);
            _controller = new TypeProduitController(mapperWrapper, manager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var entry in _context.ChangeTracker.Entries<TypeProduit>().ToList())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            foreach (var type in _testTypes)
            {
                var tracked = _context.TypeProduits.Local.FirstOrDefault(t => t.IdTypeProduit == type.IdTypeProduit)
                              ?? _context.TypeProduits.Find(type.IdTypeProduit);
                if (tracked != null)
                    _context.TypeProduits.Remove(tracked);
            }
            _context.SaveChanges();
            _testTypes.Clear();
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnAllTypeProduits()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var types = okResult!.Value as IEnumerable<TypeProduitDto>;

            Assert.IsNotNull(types);
            Assert.AreEqual(_testTypes.Count, types.Count() - _initialTypeCount);
            Assert.IsTrue(types.Any(t => t.NomTypeProduit == "Electromenager"));
            Assert.IsTrue(types.Any(t => t.NomTypeProduit == "Meuble"));
        }

        [TestMethod]
        public async Task GetById_ShouldReturnTypeProduitDto_WhenExists()
        {
            var type = _testTypes[0];

            var result = await _controller.Get(type.IdTypeProduit);
            var dto = result.Value;

            Assert.IsNotNull(dto);
            Assert.AreEqual(type.NomTypeProduit, dto.NomTypeProduit);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            var result = await _controller.Get(999);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ShouldAddTypeProduit()
        {
            var dto = new TypeProduitDto { NomTypeProduit = "Bureau" };

            var result = await _controller.Create(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var typeInDb = _context.TypeProduits.FirstOrDefault(t => t.NomTypeProduit == "Bureau");
            Assert.IsNotNull(typeInDb);

            // Nettoyage
            _context.TypeProduits.Remove(typeInDb);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task Update_ShouldModifyExistingTypeProduit()
        {
            var type = _testTypes[0];
            var updated = new TypeProduitDto { IdTypeProduit = type.IdTypeProduit, NomTypeProduit = "ElectromenagerModifie" };

            var result = await _controller.Update(type.IdTypeProduit, updated);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var inDb = _context.TypeProduits.Find(type.IdTypeProduit);
            Assert.AreEqual("ElectromenagerModifie", inDb.NomTypeProduit);
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenTypeProduitDoesNotExist()
        {
            var updated = new TypeProduitDto { IdTypeProduit = 999, NomTypeProduit = "Inexistant" };
            var result = await _controller.Update(999, updated);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveTypeProduit()
        {
            var type = _testTypes[0];

            var result = await _controller.Delete(type.IdTypeProduit);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var deleted = _context.TypeProduits.Find(type.IdTypeProduit);
            Assert.IsNull(deleted);

            _testTypes.Remove(type);
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_WhenTypeProduitDoesNotExist()
        {
            var result = await _controller.Delete(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
