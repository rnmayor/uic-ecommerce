using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Stores.Brands.Queries.GetAllStoreBrands;
using Ecommerce.Domain.Common;
using Ecommerce.TestUtils.Attributes;
using System.Net;

namespace Ecommerce.Application.Tests.Admin.Stores.Brands.Queries.GetAllStoreBrands
{
    public sealed class GetAllStoreBrandsServiceTests
    {
        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsStoreBrandList(
            [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
            GetAllStoreBrandsService service)
        {
            // Arrange
            var query = new GetAllBrandsQuery { Skip = 0, Limit = 10 };
            var storeBrands = new List<StoreBrandDTO>
        {
            new() { BrandId = Guid.NewGuid(), Name = "Brand A" },
            new() { BrandId = Guid.NewGuid(), Name = "Brand B" }
        };
            int totalCount = 2;

            repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((storeBrands, totalCount));

            // Act
            var result = await service.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(totalCount, result.Value.Total);
            Assert.Equal(2, result.Value.Brands.Count);
            Assert.Same(storeBrands, result.Value.Brands);

            repositoryMock.Verify(r => r.GetAllAsync(
                It.Is<GetAllBrandsQuery>(q => q.Limit == 10),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_SanitizeLimit(
            [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
            GetAllStoreBrandsService service
        )
        {
            // Arrange
            var query = new GetAllBrandsQuery { Limit = 9999 };
            repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<StoreBrandDTO>(), 0));

            // Act
            var result = await service.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            repositoryMock.Verify(r => r.GetAllAsync(
                It.Is<GetAllBrandsQuery>(q => q.Limit == 100),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_SanitizeSkip(
            [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
            GetAllStoreBrandsService service
        )
        {
            // Arrange
            var query = new GetAllBrandsQuery { Skip = -5 };
            repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<StoreBrandDTO>(), 0));

            // Act
            var result = await service.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            repositoryMock.Verify(r => r.GetAllAsync(
                It.Is<GetAllBrandsQuery>(q => q.Skip == 0),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsEmptyList(
            [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
            GetAllStoreBrandsService service
        )
        {
            // Arrange
            var query = new GetAllBrandsQuery();
            repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<StoreBrandDTO>(), 0));

            // Act
            var result = await service.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value.Brands);
            Assert.Equal(0, result.Value.Total);

            repositoryMock.Verify(r => r.GetAllAsync(
                It.IsAny<GetAllBrandsQuery>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_PassesCancellationToken(
            [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
            GetAllStoreBrandsService service
        )
        {
            using var cts = new CancellationTokenSource();
            var query = new GetAllBrandsQuery();

            repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), cts.Token))
                .ReturnsAsync((new List<StoreBrandDTO>(), 0));

            var result = await service.HandleAsync(query, cts.Token);

            Assert.True(result.IsSuccess);
            repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), cts.Token), Times.Once);
        }
    }
}