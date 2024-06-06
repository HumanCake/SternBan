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
        //TODO: implement format validation
        if (string.IsNullOrEmpty(board.BoardId))
        {
            return BadRequest("invalid format");
        }
        var result = await _kanbanService.PutBoardAsync(board);
        _logger.LogInformation($"The Board with ID '{result.BoardId}' was created or updated");

        return Ok("The board was created or updated:\n" + result);
    }
}