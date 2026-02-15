using Ecommerce.Application.Admin.Stores.Brands.GetAll;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Stores.Brands;

[ApiController]
[Route("/api/admin/store-brands")]
[Authorize]
[SkipTenantResolution]
public sealed class GetAllStoreBrandsController : ControllerBase
{
    private readonly IGetAllStoreBrandsService _service;
    public GetAllStoreBrandsController(IGetAllStoreBrandsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<StoreBrandsResponse>> HandleAsync(CancellationToken ct)
    {
        return Ok(await _service.HandleAsync(ct));
    }

}
