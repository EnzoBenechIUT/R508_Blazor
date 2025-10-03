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
    public class ProductControllerTests
    {
        private AppDbContext _context;
        private ProductController _controller;
        private IMapper _mapper;
        private List<Produit> _testProduits;
        private int _initialProductCount;
        [TestInitialize]
        public void Initialize()
        {
            // Création du contexte
            _context = new AppDbContext();

            // AutoMapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _initialProductCount = _context.Produits.Count();

            // Création de produits de test
            _testProduits = new List<Produit>
            {
                new Produit
                {
                    NomProduit = "Chaise",
                    Description = "Description chaise",
                    NomPhoto = "chaise.jpg",
                    UriPhoto = "http://test.com/chaise.jpg",
                    StockReel = 5,
                    StockMin = 1,
                    StockMax = 10
                },
                new Produit
                {
                    NomProduit = "Table",
                    Description = "Description table",
                    NomPhoto = "table.jpg",
                    UriPhoto = "http://test.com/table.jpg",
                    StockReel = 3,
                    StockMin = 1,
                    StockMax = 5
                }
            };
            _context.Produits.AddRange(_testProduits);
            _context.SaveChanges();

            // Controller
            var mapperWrapper = new AutoMapperWrapper(_mapper);
            var manager = new ProductManager(_context);
            _controller = new ProductController(mapperWrapper, manager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Supprime uniquement les produits créés pour les tests
            _context.Produits.RemoveRange(_testProduits);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task GetById_ShouldReturnProduitDto_WhenProduitExists()
        {
            var produit = _testProduits[0];

            var result = await _controller.Get(produit.IdProduit);
            var dto = result.Value;

            Assert.IsNotNull(dto);
            Assert.AreEqual(produit.NomProduit, dto.Nom);
            Assert.AreEqual(produit.Description, dto.Description);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenProduitDoesNotExist()
        {
            var result = await _controller.Get(999);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnAllProducts()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var produits = okResult!.Value as IEnumerable<ProduitDto>;

            Assert.IsNotNull(produits);
            Assert.AreEqual(_testProduits.Count, produits.Count() - _initialProductCount);
            Assert.IsTrue(produits.Any(p => p.Nom == "Chaise"));
            Assert.IsTrue(produits.Any(p => p.Nom == "Table"));
        }

        [TestMethod]
        public async Task Create_ShouldAddProduct()
        {
            var dto = new ProduitDto
            {
                Nom = "Bureau",
                Description = "Description bureau",
                NomPhoto = "bureau.jpg",
                UriPhoto = "http://test.com/bureau.jpg",
                StockReel = 8,
                StockMin = 2,
                StockMax = 15
            };

            var result = await _controller.Create(dto);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var produitInDb = _context.Produits.FirstOrDefault(p => p.NomProduit == "Bureau");
            Assert.IsNotNull(produitInDb);

            // Nettoyage
            _context.Produits.Remove(produitInDb);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task Update_ShouldModifyExistingProduct()
        {
            var produit = _testProduits[0];
            var dto = new ProduitDto
            {
                Id = produit.IdProduit,
                Nom = "ChaiseModifiee",
                Description = produit.Description,
                NomPhoto = produit.NomPhoto,
                UriPhoto = produit.UriPhoto,
                StockReel = produit.StockReel,
                StockMin = produit.StockMin,
                StockMax = produit.StockMax
            };

            var result = await _controller.Update(produit.IdProduit, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var updated = _context.Produits.Find(produit.IdProduit);
            Assert.AreEqual("ChaiseModifiee", updated.NomProduit);
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var dto = new ProduitDto { Id = 999, Nom = "Inexistant" };
            var result = await _controller.Update(999, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveProduct()
        {
            var produit = _testProduits[0];

            var result = await _controller.Delete(produit.IdProduit);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var deleted = _context.Produits.Find(produit.IdProduit);
            Assert.IsNull(deleted);

            // Retirer de la liste pour ne pas le supprimer à nouveau dans Cleanup
            _testProduits.Remove(produit);
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var result = await _controller.Delete(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }

    // Wrapper pour utiliser AutoMapper dans le controller
    public class AutoMapperWrapper : App.Mapper.IMapper<Produit, ProduitDto>
    {
        private readonly IMapper _mapper;
        public AutoMapperWrapper(IMapper mapper) => _mapper = mapper;
        public ProduitDto ToDTO(Produit entity) => _mapper.Map<ProduitDto>(entity);
        public Produit ToEntity(ProduitDto dto) => _mapper.Map<Produit>(dto);
        public IEnumerable<ProduitDto> ToDTOs(IEnumerable<Produit> entities) => _mapper.Map<IEnumerable<ProduitDto>>(entities);
    }
}
