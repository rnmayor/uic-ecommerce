using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Tenants.Queries.GetMyTenants;
using Ecommerce.Domain.Tenants;
using Ecommerce.TestUtils.Attributes;

namespace Ecommerce.Application.Tests.Admin.Tenants.Queries.GetMyTenants
{
    public sealed class GetMyTenantsServiceTests
    {
        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsTenantList_WhenUserHasMemberships(
            Guid userId,
            [Frozen] Mock<IGetTenantsForUserRepository> repositoryMock,
            GetMyTenantsService service)
        {
            // Arrange
            var tenants = new List<MyTenantDTO>
        {
            new() { TenantId = Guid.NewGuid(), Name = "Tenant A", IsOwner = true, Role = TenantRoles.Owner },
            new() { TenantId = Guid.NewGuid(), Name = "Tenant B", IsOwner = false, Role = TenantRoles.Admin }
        };

            repositoryMock
                .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tenants);

            // Act
            var result = await service.HandleAsync(userId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Tenants.Count);
            Assert.True(result.Value.HasTenant);
            Assert.Equal(tenants, result.Value.Tenants);

            repositoryMock.Verify(r => r.GetTenantsForUserAsync(
                userId, It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_ReturnsEmptyList_WhenUserHasNoTenants(
            Guid userId,
            [Frozen] Mock<IGetTenantsForUserRepository> repositoryMock,
            GetMyTenantsService service)
        {
            // Arrange
            repositoryMock
                .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<MyTenantDTO>());

            // Act
            var result = await service.HandleAsync(userId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value.Tenants);
            Assert.False(result.Value.HasTenant);

            repositoryMock.Verify(r => r.GetTenantsForUserAsync(
                userId, It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory]
        [InlineAutoMoqData(TenantRoles.Admin)]
        [InlineAutoMoqData(TenantRoles.Manager)]
        [InlineAutoMoqData(TenantRoles.Staff)]
        [InlineAutoMoqData(TenantRoles.Customer)]
        public async Task HandleAsync_IsOwnerIsFalse_ForNonOwnerRoles(
            string role,
            Guid userId,
            [Frozen] Mock<IGetTenantsForUserRepository> repositoryMock,
            GetMyTenantsService service)
        {
            // Arrange
            var tenants = new List<MyTenantDTO>
        {
            new() { TenantId = Guid.NewGuid(), Name = "Tenant", Role = role, IsOwner = false }
        };

            repositoryMock
                .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tenants);

            // Act
            var result = await service.HandleAsync(userId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Value.Tenants.Single().IsOwner);

            repositoryMock.Verify(r => r.GetTenantsForUserAsync(
                userId, It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task HandleAsync_PassesCancellationToken(
            Guid userId,
            [Frozen] Mock<IGetTenantsForUserRepository> repositoryMock,
            GetMyTenantsService service)
        {
            using var cts = new CancellationTokenSource();

            repositoryMock
                .Setup(r => r.GetTenantsForUserAsync(userId, cts.Token))
                .ReturnsAsync(new List<MyTenantDTO>());

            var result = await service.HandleAsync(userId, cts.Token);

            Assert.True(result.IsSuccess);
            repositoryMock.Verify(r => r.GetTenantsForUserAsync(It.IsAny<Guid>(), cts.Token), Times.Once);
        }
    }
}