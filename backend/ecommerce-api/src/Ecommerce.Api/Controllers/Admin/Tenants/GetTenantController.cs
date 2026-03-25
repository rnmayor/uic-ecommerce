using Ecommerce.Application.Admin.Tenants.Queries.GetTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Tenants
{
    [Route("api/admin/tenants")]
    [Authorize]
    public class GetTenantController : ApiController
    {
        private readonly IGetTenantService _service;
        public GetTenantController(IGetTenantService service)
        {
            _service = service;
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<GetTenantResponse>> HandleAsync(string slug, CancellationToken ct)
        {
            var result = await _service.HandleAsync(slug, ct);

            return HandleResult(result, tenant => Ok(tenant));
        }
    }
}
