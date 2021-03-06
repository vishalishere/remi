using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using ReMi.Common.Utils.Repository;
using ReMi.TestUtils.UnitTests;
using ReMi.DataAccess.BusinessEntityGateways.Products;
using ReMi.DataAccess.Exceptions;
using ReMi.DataEntities.Auth;
using ReMi.DataEntities.Products;

namespace ReMi.DataAccess.Tests.Products
{
    [TestFixture]
    public class BusinessUnitsGatewayTests : TestClassFor<BusinessUnitsGateway>
    {
        private Mock<IRepository<Product>> _packageRepositoryMock;
        private Mock<IRepository<Account>> _accountRepositoryMock;
        private Mock<IRepository<BusinessUnit>> _businessUnitRepositoryMock;
        private Mock<IMappingEngine> _mappingEngineMock;

        protected override BusinessUnitsGateway ConstructSystemUnderTest()
        {
            _packageRepositoryMock = new Mock<IRepository<Product>>(MockBehavior.Strict);
            _accountRepositoryMock = new Mock<IRepository<Account>>(MockBehavior.Strict);
            _businessUnitRepositoryMock = new Mock<IRepository<BusinessUnit>>(MockBehavior.Strict);
            _mappingEngineMock = new Mock<IMappingEngine>(MockBehavior.Strict);

            return new BusinessUnitsGateway
            {
                PackageRepository = _packageRepositoryMock.Object,
                AccountRepository = _accountRepositoryMock.Object,
                BusinessUnitRepository = _businessUnitRepositoryMock.Object,
                MappingEngine = _mappingEngineMock.Object
            };
        }

        [Test]
        public void OnDispose_ShouldDisposeAllRepositories_WhenFinished()
        {
            _accountRepositoryMock.Setup(x => x.Dispose());
            _businessUnitRepositoryMock.Setup(x => x.Dispose());
            _packageRepositoryMock.Setup(x => x.Dispose());

            using (Sut) { }

            _accountRepositoryMock.Verify(x => x.Dispose(), Times.Once);
            _businessUnitRepositoryMock.Verify(x => x.Dispose(), Times.Once);
            _packageRepositoryMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        [ExpectedException(typeof(AccountNotFoundException))]
        public void GetPackages_ShouldThrowAccountNotFound_WhenAccoundNotExists()
        {
            _accountRepositoryMock.SetupEntities(Enumerable.Empty<Account>());

            Sut.GetPackages(Guid.NewGuid());
        }

        [Test]
        public void GetPackages_ShouldGetPackagesAssignToRole_WhenUserIsNotAdmin()
        {
            var packages = Builder<Product>.CreateListOfSize(5)
                .All()
                .Do(x => x.ExternalId = Guid.NewGuid())
                .Do(x => x.AccountProducts = new[]
                {
                    new AccountProduct { AccountId = RandomData.RandomInt(int.MaxValue) }
                }).Build();
            var account = new Account
            {
                ExternalId = Guid.NewGuid(),
                Role = new Role { Name = "ProductOwner" },
                AccountId = RandomData.RandomInt(int.MaxValue)
            };
            packages.ElementAt(2).AccountProducts.First().AccountId = account.AccountId;
            packages.ElementAt(4).AccountProducts.First().AccountId = account.AccountId;
            _accountRepositoryMock.SetupEntities(new[] { account });
            _packageRepositoryMock.SetupEntities(packages);
            _mappingEngineMock.Setup(x => x.Map<IEnumerable<Product>, IEnumerable<BusinessEntities.Products.Product>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<BusinessEntities.Products.Product>)null);

            Sut.GetPackages(account.ExternalId);

            _mappingEngineMock.Verify(
                x =>
                    x.Map<IEnumerable<Product>, IEnumerable<BusinessEntities.Products.Product>>(
                        It.Is<IEnumerable<Product>>(p => p.Count() == 2)));

        }

        [Test]
        public void GetPackages_ShouldGetAllPackages_WhenUserIsAdmin()
        {
            var packages = Builder<Product>.CreateListOfSize(5)
                .All()
                .Do(x => x.ExternalId = Guid.NewGuid())
                .Do(x => x.AccountProducts = new[]
                {
                    new AccountProduct { AccountId = RandomData.RandomInt(int.MaxValue) }
                }).Build();
            var account = new Account
            {
                ExternalId = Guid.NewGuid(),
                Role = new Role { Name = "Admin" },
                AccountId = RandomData.RandomInt(int.MaxValue)
            };
            packages.ElementAt(2).AccountProducts.First().AccountId = account.AccountId;
            packages.ElementAt(4).AccountProducts.First().AccountId = account.AccountId;
            _accountRepositoryMock.SetupEntities(new[] { account });
            _packageRepositoryMock.SetupEntities(packages);
            _mappingEngineMock.Setup(x => x.Map<IEnumerable<Product>, IEnumerable<BusinessEntities.Products.Product>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<BusinessEntities.Products.Product>)null);

            Sut.GetPackages(account.ExternalId);

            _mappingEngineMock.Verify(
                x =>
                    x.Map<IEnumerable<Product>, IEnumerable<BusinessEntities.Products.Product>>(
                        It.Is<IEnumerable<Product>>(p => p.Count() == 5)));

        }

        [Test]
        public void GetPackages_ShouldGetAllPackages_WhenRequestedIncludeAll()
        {
            var packages = Builder<Product>.CreateListOfSize(5)
                .All()
                .Do(x => x.ExternalId = Guid.NewGuid())
                .Do(x => x.AccountProducts = new[]
                {
                    new AccountProduct { AccountId = RandomData.RandomInt(int.MaxValue) }
                }).Build();
            var account = new Account
            {
                ExternalId = Guid.NewGuid(),
                Role = new Role { Name = "ProductOwner" },
                AccountId = RandomData.RandomInt(int.MaxValue)
            };
            packages.ElementAt(2).AccountProducts.First().AccountId = account.AccountId;
            packages.ElementAt(4).AccountProducts.First().AccountId = account.AccountId;
            _accountRepositoryMock.SetupEntities(new[] { account });
            _packageRepositoryMock.SetupEntities(packages);
            _mappingEngineMock.Setup(x => x.Map<IEnumerable<Product>, IEnumerable<BusinessEntities.Products.Product>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<BusinessEntities.Products.Product>)null);

            Sut.GetPackages(account.ExternalId, true);

            _mappingEngineMock.Verify(
                x =>
                    x.Map<IEnumerable<Product>, IEnumerable<BusinessEntities.Products.Product>>(
                        It.Is<IEnumerable<Product>>(p => p.Count() == 5)));

        }

        [Test]
        [ExpectedException(typeof(AccountNotFoundException))]
        public void GetDefaultPackage_ShouldThrowAccountNotFound_WhenAccoundNotExists()
        {
            _accountRepositoryMock.SetupEntities(Enumerable.Empty<Account>());

            Sut.GetDefaultPackage(Guid.NewGuid());
        }


        [Test]
        public void GetDefaultPackage_ShouldGetDefaultPackage_WhenDefaultPackageExists()
        {

            var account = new Account
            {
                ExternalId = Guid.NewGuid(),
                AccountProducts = Builder<AccountProduct>.CreateListOfSize(5)
                    .All()
                    .Do(x => x.IsDefault = false).Build()
            };
            var defaultPackage = new Product { ExternalId = Guid.NewGuid() };
            account.AccountProducts.ElementAt(4).IsDefault = true;
            account.AccountProducts.ElementAt(4).Product = defaultPackage;
            _accountRepositoryMock.SetupEntities(new[] { account });
            _mappingEngineMock.Setup(x => x.Map<Product, BusinessEntities.Products.Product>(defaultPackage))
                .Returns((BusinessEntities.Products.Product)null);

            Sut.GetDefaultPackage(account.ExternalId);

            _mappingEngineMock.Verify(x => x.Map<Product, BusinessEntities.Products.Product>(It.IsAny<Product>()), Times.Once);

        }

        [Test]
        public void GetDefaultPackage_ShouldGetFirstPackage_WhenDefaultPackageNotExists()
        {

            var account = new Account
            {
                ExternalId = Guid.NewGuid(),
                AccountProducts = Builder<AccountProduct>.CreateListOfSize(5)
                    .All()
                    .Do(x => x.IsDefault = false).Build()
            };
            var firstPackage = new Product { ExternalId = Guid.NewGuid() };
            account.AccountProducts.First().Product = firstPackage;
            _accountRepositoryMock.SetupEntities(new[] { account });
            _mappingEngineMock.Setup(x => x.Map<Product, BusinessEntities.Products.Product>(firstPackage))
                .Returns((BusinessEntities.Products.Product)null);

            Sut.GetDefaultPackage(account.ExternalId);

            _mappingEngineMock.Verify(x => x.Map<Product, BusinessEntities.Products.Product>(It.IsAny<Product>()), Times.Once);

        }

        [Test]
        public void GetDefaultPackage_ShouldGetNull_WhenAccountDoesNotHaveAnyPackages()
        {

            var account = new Account
            {
                ExternalId = Guid.NewGuid(),
                AccountProducts = new List<AccountProduct>()
            };
            _accountRepositoryMock.SetupEntities(new[] { account });

            Sut.GetDefaultPackage(account.ExternalId);

            _mappingEngineMock.Verify(x => x.Map<Product, BusinessEntities.Products.Product>(It.IsAny<Product>()), Times.Never);

        }

        [Test]
        public void GetEmptyBusinessUnits_ShouldGetAllBusinessUnits_WhenBusinessUnitDoesNotHavePackages()
        {
            var businessUnits = new[]
            {
                new BusinessUnit { ExternalId = Guid.NewGuid(), Packages = new [] { new Product() } },
                new BusinessUnit { ExternalId = Guid.NewGuid(), Packages = new Product[0] }
            };
            _mappingEngineMock.Setup(x => x.Map<IEnumerable<BusinessUnit>, IEnumerable<BusinessEntities.Products.BusinessUnit>>(
                It.Is<IEnumerable<BusinessUnit>>(b => b.Count() == 1)))
                .Returns((IEnumerable<BusinessEntities.Products.BusinessUnit>)null);

            _businessUnitRepositoryMock.SetupEntities(businessUnits);

            Sut.GetEmptyBusinessUnits();

            _mappingEngineMock.Verify(x => x.Map<IEnumerable<BusinessUnit>, IEnumerable<BusinessEntities.Products.BusinessUnit>>(
                It.IsAny<IEnumerable<BusinessUnit>>()), Times.Once);

        }

        [Test]
        public void AddBusinessUnit_ShouldThrowException_WhenExternalIdAlreadyExists()
        {
            var businessUnits = Builder<BusinessUnit>.CreateListOfSize(5).Build();
            var businessUnit = Builder<BusinessEntities.Products.BusinessUnit>.CreateNew()
                .With(x => x.ExternalId, businessUnits[0].ExternalId)
                .Build();

            _businessUnitRepositoryMock.SetupEntities(businessUnits);

            var ex = Assert.Throws<EntityAlreadyExistsException>(() => Sut.AddBusinessUnit(businessUnit));

            Assert.IsTrue(ex.Message.Contains(businessUnit.ExternalId.ToString()));
        }

        [Test]
        public void AddBusinessUnit_ShouldAddNewBusinnessUnit_WhenSucced()
        {
            var businessUnits = Builder<BusinessUnit>.CreateListOfSize(5).Build();
            var businessUnit = Builder<BusinessEntities.Products.BusinessUnit>.CreateNew()
                .With(x => x.ExternalId, Guid.NewGuid())
                .Build();
            var dataBusinessUnit = new BusinessUnit { ExternalId = businessUnit.ExternalId };

            _businessUnitRepositoryMock.SetupEntities(businessUnits);
            _mappingEngineMock.Setup(x => x.Map<BusinessEntities.Products.BusinessUnit, BusinessUnit>(businessUnit))
                .Returns(dataBusinessUnit);
            _businessUnitRepositoryMock.Setup(x => x.Insert(dataBusinessUnit));

            Sut.AddBusinessUnit(businessUnit);

            _mappingEngineMock.Verify(x => x.Map<BusinessEntities.Products.BusinessUnit, BusinessUnit>(businessUnit), Times.Once);
            _businessUnitRepositoryMock.Verify(x => x.Insert(dataBusinessUnit), Times.Once);
        }

        [Test]
        public void UpdateBusinessUnit_ShouldThrowException_WhenExternalIdNotExists()
        {
            var businessUnits = Builder<BusinessUnit>.CreateListOfSize(5).Build();
            var businessUnit = Builder<BusinessEntities.Products.BusinessUnit>.CreateNew()
                .With(x => x.ExternalId, Guid.NewGuid())
                .Build();

            _businessUnitRepositoryMock.SetupEntities(businessUnits);

            var ex = Assert.Throws<EntityNotFoundException>(() => Sut.UpdateBusinessUnit(businessUnit));

            Assert.IsTrue(ex.Message.Contains(businessUnit.ExternalId.ToString()));
        }

        [Test]
        public void UpdateBusinessUnit_ShouldUpdateBusinnessUnit_WhenSucced()
        {
            var businessUnits = Builder<BusinessUnit>.CreateListOfSize(5).Build();
            var businessUnit = Builder<BusinessEntities.Products.BusinessUnit>.CreateNew()
                .With(x => x.ExternalId, businessUnits[3].ExternalId)
                .Build();

            _businessUnitRepositoryMock.SetupEntities(businessUnits);
            _businessUnitRepositoryMock.Setup(x => x.Update(businessUnits[3])).Returns((ChangedFields<BusinessUnit>)null);

            Sut.UpdateBusinessUnit(businessUnit);

            Assert.AreEqual(businessUnits[3].Name, businessUnit.Name);
            Assert.AreEqual(businessUnits[3].Description, businessUnit.Description);
            _businessUnitRepositoryMock.Verify(x => x.Update(businessUnits[3]), Times.Once);
        }

        [Test]
        public void RemoveBusinessUnit_ShouldThrowException_WhenExternalIdNotExists()
        {
            var businessUnits = Builder<BusinessUnit>.CreateListOfSize(5).Build();
            var businessUnitId = Guid.NewGuid();

            _businessUnitRepositoryMock.SetupEntities(businessUnits);

            var ex = Assert.Throws<EntityNotFoundException>(() => Sut.RemoveBusinessUnit(businessUnitId));

            Assert.IsTrue(ex.Message.Contains(businessUnitId.ToString()));
        }


        [Test]
        public void RemoveBusinessUnit_ShouldThrowException_WhenBusinessUnitHasPackages()
        {
            var businessUnits = Builder<BusinessUnit>.CreateListOfSize(5)
                .Section(3, 4)
                .With(x => x.Packages, Builder<Product>.CreateListOfSize(5).Build())
                .Build();
            var businessUnitId = businessUnits[3].ExternalId;

            _businessUnitRepositoryMock.SetupEntities(businessUnits);

            var ex = Assert.Throws<EntityHasRelatedData>(() => Sut.RemoveBusinessUnit(businessUnitId));

            Assert.IsTrue(ex.Message.Contains(businessUnitId.ToString()));
        }

        [Test]
        public void RemoveBusinessUnit_ShouldAddNewBusinnessUnit_WhenSucced()
        {
            var businessUnits = Builder<BusinessUnit>.CreateListOfSize(5)
                .Section(3, 4)
                .With(x => x.Packages, Enumerable.Empty<Product>())
                .Build();
            var businessUnitId = businessUnits[3].ExternalId;

            _businessUnitRepositoryMock.SetupEntities(businessUnits);
            _businessUnitRepositoryMock.Setup(x => x.Delete(businessUnits[3]));

            Sut.RemoveBusinessUnit(businessUnitId);

            _businessUnitRepositoryMock.Verify(x => x.Delete(businessUnits[3]), Times.Once);
        }
    }
}
