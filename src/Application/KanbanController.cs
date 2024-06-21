using Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Application;

[Route("api/[controller]")]
[ApiController]
public class KanbanController : ControllerBase
{
    private readonly IValidator<Board> _boardValidator;
    private readonly IKanbanService _kanbanService;
    private readonly ILogger<KanbanController> _logger;

    public KanbanController(ILogger<KanbanController> logger, IKanbanService kanbanService,
        IValidator<Board> boardValidator)
    {
        _kanbanService = kanbanService;
        _boardValidator = boardValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetBoard(string boardId)
    {
        var board = await _kanbanService.GetBoardAsync(boardId);
        if (!board.Success) BadRequest(board.ErrorMessage);

        _logger.LogInformation($"Board with ID '{boardId}' retrieved successfully.");

        return Ok(board.Data);
    }

    [HttpPut]
    public async Task<IActionResult> PutBoard(Board board)
    {
        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _kanbanService.PutBoardAsync(board);
        if (result.Data != null)
        {
            _logger.LogInformation($"The Board with ID '{result.Data.BoardId}' was created or updated");
            return Ok("The board was created or updated:\n" + result.Data);
        }

        return StatusCode(500);

    }
}