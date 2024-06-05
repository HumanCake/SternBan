using Microsoft.AspNetCore.Mvc;

namespace Application;

[Route("api/[controller]")]
[ApiController]
public class KanbanController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var data = new { Message = "Hello, World!" };

        return Ok(data);
    }
}