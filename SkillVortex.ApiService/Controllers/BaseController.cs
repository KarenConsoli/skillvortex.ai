using Microsoft.AspNetCore.Mvc;

namespace SkillVortex.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
    }
}