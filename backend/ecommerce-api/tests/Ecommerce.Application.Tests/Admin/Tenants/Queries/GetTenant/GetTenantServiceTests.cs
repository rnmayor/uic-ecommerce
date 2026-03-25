using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Tenants.Queries.GetTenant;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Tenants;
using Ecommerce.TestUtils.Attributes;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Tests.Admin.Tenants.Queries.GetTenant
{
    public sealed class GetTenantServiceTests
    {
        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsTenant_WhenValid(
            Guid tenantId,
            [Frozen] Mock<IGetTenantRepository> repositoryMock,
            [Frozen] Mock<ITenantContext> tenantContextMock,
            [Frozen] Mock<ILogger<GetTenantService>> loggerMock,
            GetTenantService service)
        {
            // Arrange
            var validSlug = "valid-tenant-slug";
            tenantContextMock.SetupGet(t => t.TenantId).Returns(tenantId);

            var response = new GetTenantResponse
            {
                Tenant = new GetTenantDTO
                {
                    Id = tenantId,
                    Name = "Valid Tenant Slug",
                    Slug = validSlug
                },
                Stores = new List<GetTenantStoreDTO>
                {
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 1", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 2", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 3", StoreBrandName = "Brand 2"}
                }
            };

            repositoryMock
                .Setup(r => r.GetTenantAsync(tenantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await service.HandleAsync(validSlug, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            Assert.Equal(response.Tenant.Id, result.Value.Tenant.Id);
            Assert.Equal(response.Tenant.Slug, result.Value.Tenant.Slug);
            Assert.Equal(response.Tenant.Name, result.Value.Tenant.Name);

            Assert.Equal(response.Stores.Count, result.Value.Stores.Count);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            , Times.Never);

            tenantContextMock.Verify(t => t.TenantId, Times.Once);

            repositoryMock.Verify(r => r.GetTenantAsync(
                tenantId, It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsFailure_WhenSlugIsNotCanonical(
            [Frozen] Mock<IGetTenantRepository> repositoryMock,
            [Frozen] Mock<ITenantContext> tenantContextMock,
            GetTenantService service)
        {
            // Arrange
            var invalidSlug = "---invalid---slug";

            // Act
            var result = await service.HandleAsync(invalidSlug, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(TenantErrors.InvalidSlug.Code, result.Error.Code);

            tenantContextMock.Verify(t => t.TenantId, Times.Never);
            repositoryMock.Verify(r => r.GetTenantAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsFailure_WhenRepositoryIsNull(
            Guid tenantId,
            [Frozen] Mock<IGetTenantRepository> repositoryMock,
            [Frozen] Mock<ITenantContext> tenantContextMock,
            GetTenantService service)
        {
            // Arrange
            var validSlug = "valid-tenant-slug";
            tenantContextMock.SetupGet(x => x.TenantId).Returns(tenantId);

            repositoryMock
                .Setup(r => r.GetTenantAsync(tenantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTenantResponse?)null);

            // Act
            var result = await service.HandleAsync(validSlug, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(TenantErrors.TenantNotFound.Code, result.Error.Code);

            tenantContextMock.Verify(t => t.TenantId, Times.Once);
            repositoryMock.Verify(r => r.GetTenantAsync(tenantId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsFailure_WhenSlugMismatch(
            Guid tenantId,
            [Frozen] Mock<IGetTenantRepository> repositoryMock,
            [Frozen] Mock<ITenantContext> tenantContextMock,
            [Frozen] Mock<ILogger<GetTenantService>> loggerMock,
            GetTenantService service)
        {
            // Arrange
            var requestedSlug = "requested-slug";
            var actualSlug = "actual-slug";

            tenantContextMock.SetupGet(x => x.TenantId).Returns(tenantId);

            var response = new GetTenantResponse
            {
                Tenant = new GetTenantDTO
                {
                    Id = tenantId,
                    Name = "Actual Slug",
                    Slug = actualSlug
                },
                Stores = new List<GetTenantStoreDTO>
                {
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 1", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 2", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 3", StoreBrandName = "Brand 2"}
                }
            };

            repositoryMock
                .Setup(r => r.GetTenantAsync(tenantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await service.HandleAsync(requestedSlug, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(TenantErrors.TenantNotFound.Code, result.Error.Code);

            tenantContextMock.Verify(t => t.TenantId, Times.Once());
            repositoryMock.Verify(r => r.GetTenantAsync(tenantId, It.IsAny<CancellationToken>()), Times.Once);
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Potential URL tampering")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            , Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_PassesCancellationToken(
            Guid tenantId,
            [Frozen] Mock<ITenantContext> tenantContextMock,
            [Frozen] Mock<IGetTenantRepository> repositoryMock,
            GetTenantService service)
        {
            // Arrange
            var validSlug = "tenant-name";
            tenantContextMock.SetupGet(t => t.TenantId).Returns(tenantId);

            var response = new GetTenantResponse
            {
                Tenant = new GetTenantDTO
                {
                    Id = tenantId,
                    Name = "Tenant Name",
                    Slug = validSlug
                },
                Stores = new List<GetTenantStoreDTO>()
            };

            var cts = new CancellationTokenSource();
            repositoryMock
                .Setup(r => r.GetTenantAsync(tenantId, cts.Token))
                .ReturnsAsync(response);

            // Act
            var result = await service.HandleAsync(validSlug, cts.Token);

            // Assert
            Assert.True(result.IsSuccess);

            tenantContextMock.Verify(t => t.TenantId, Times.Once);
            repositoryMock.Verify(r => r.GetTenantAsync(tenantId, cts.Token), Times.Once);
        }
    }
}
