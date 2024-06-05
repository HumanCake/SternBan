using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Application;

[Route("api/[controller]")]
[ApiController]
public class KanbanController : ControllerBase
{
    private readonly IKanbanService _kanbanService;
    private readonly ILogger<KanbanController> _logger;

    public KanbanController(ILogger<KanbanController> logger, IKanbanService kanbanService)
    {
        _logger = logger;
        _kanbanService = kanbanService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBoard(string boardId)
    {
        var board = await _kanbanService.GetBoardAsync(boardId);
        if (board == null)
        {
            _logger.LogWarning($"Board with ID '{boardId}' not found.");
            return NotFound();
        }

        _logger.LogInformation($"Board with ID '{boardId}' retrieved successfully.");

        return Ok(board);
    }

    [HttpPut]
    public async Task<IActionResult> PutBoard(Board board)
    {
        var result = await _kanbanService.PutBoardAsync(board);

        return Ok("The board was created or updated" + result);
    }
}