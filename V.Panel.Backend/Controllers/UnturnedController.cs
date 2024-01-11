using Microsoft.AspNetCore.Mvc;

namespace V.Panel.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
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
