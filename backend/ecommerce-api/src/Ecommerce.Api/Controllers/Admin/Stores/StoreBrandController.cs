using Ecommerce.Application.Admin.Stores.Brands;
using Ecommerce.Application.Common.Attributes;
using Ecommerce.Application.Stores.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Stores;

[ApiController]
[Route("/api/admin/store-brands")]
[Authorize]
[SkipTenantResolution]
public sealed class StoreBrandController : ControllerBase
{
    private readonly IStoreBrandService _service;
    public StoreBrandController(IStoreBrandService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<StoreBrandResponse>> GetAllAsync(CancellationToken ct)
    {
        return Ok(await _service.GetAllAsync(ct));
    }

}
