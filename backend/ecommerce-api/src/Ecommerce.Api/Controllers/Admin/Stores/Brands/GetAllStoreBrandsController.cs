using Ecommerce.Application.Admin.Stores.Brands.Queries.GetAllStoreBrands;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Stores.Brands
{
    [Route("/api/admin/store-brands")]
    [Authorize]
    [SkipTenantResolution]
    public sealed class GetAllStoreBrandsController : ApiController
    {
        private readonly IGetAllStoreBrandsService _service;
        public GetAllStoreBrandsController(IGetAllStoreBrandsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<StoreBrandsResponse>> HandleAsync(
            [FromQuery] GetAllBrandsQuery query,
            CancellationToken ct = default)
        {
            var result = await _service.HandleAsync(query, ct);
            return HandleResult(result, brands => Ok(brands));
        }

    }
}