using Ecommerce.Application.Admin.Stores.Brands.Create;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Stores.Brands;

[ApiController]
[Route("api/admin/store-brands")]
[Authorize]
[SkipTenantResolution]
public sealed class StoreBrandController : ControllerBase
{
    private readonly ICreateStorBrandService _service;
    public StoreBrandController(ICreateStorBrandService service)
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

        return CreatedAtRoute("GetStoreBrandById", new { id = result.StoreBrandId }, result);
    }

    [HttpGet("{id:guid}", Name = "GetStoreBrandById")]
    public IActionResult GetByIdAsync(Guid id)
    {
        return Ok();
    }
}
