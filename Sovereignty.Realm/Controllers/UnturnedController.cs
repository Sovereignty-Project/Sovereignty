using Microsoft.AspNetCore.Mvc;

namespace Sovereignty.Realm.Controllers;

[ApiController]
[Route("[controller]")]
public class UnturnedController : ControllerBase
{
    private readonly ILogger<UnturnedController> _logger;

    public UnturnedController(ILogger<UnturnedController> logger)
    {
        _logger = logger;
    }
    
    // [HttpGet("investigate")]
    // [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    // public ActionResult<InvestigateResponseDTO>
}
