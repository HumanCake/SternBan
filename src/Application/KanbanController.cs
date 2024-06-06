using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Application;

[Route("api/[controller]")]
[ApiController]
public class KanbanController : ControllerBase
{
    private readonly IKanbanService _kanbanService;
    private readonly IBoardValidator _boardValidator;
    private readonly ILogger<KanbanController> _logger;

    public KanbanController(ILogger<KanbanController> logger, IKanbanService kanbanService, IBoardValidator boardValidator)
    {
        _kanbanService = kanbanService;
        _boardValidator = boardValidator;
        _logger = logger;
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
        var validationResult = _boardValidator.Validate(board);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var result = await _kanbanService.PutBoardAsync(board);
        _logger.LogInformation($"The Board with ID '{result.BoardId}' was created or updated");

        return Ok("The board was created or updated:\n" + result);
    }
}