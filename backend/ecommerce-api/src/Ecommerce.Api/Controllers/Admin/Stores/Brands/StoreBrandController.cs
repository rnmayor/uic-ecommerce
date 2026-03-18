using Ecommerce.Application.Admin.Stores.Brands.Commands.CreateStoreBrand;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Stores.Brands
{
    [Route("api/admin/store-brands")]
    [Authorize]
    [SkipTenantResolution]
    public sealed class StoreBrandController : ApiController
    {
        private readonly ICreateStoreBrandService _service;
        public StoreBrandController(ICreateStoreBrandService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<CreateStoreBrandResponse>> CreateAsync(
          [FromBody] CreateStoreBrandRequest request,
          CancellationToken ct
        )
        {
            var result = await _service.ExecuteAsync(request, ct);

            return HandleResult(result, brand => CreatedAtRoute("GetStoreBrandById", new { id = brand.StoreBrandId }, brand));
        }

        [HttpGet("{id:guid}", Name = "GetStoreBrandById")]
        public IActionResult GetByIdAsync(Guid id)
        {
            return Ok();
        }
    }
}