using Microsoft.AspNetCore.Mvc;

namespace Application;

[Route("api/[controller]")]
[ApiController]
public class KanbanController : ControllerBase
{
    private ILogger<KanbanController> _logger;

    public KanbanController(ILogger<KanbanController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    public IActionResult Get()
    {
        var data = new { Message = "Hello, World!" };
        _logger.LogInformation("Get method called.");

        return Ok(data);
    }
}