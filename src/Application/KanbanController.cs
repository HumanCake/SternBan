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

    [HttpGet("{boardId}")]
    public async Task<IActionResult> GetBoard(string boardId)
    {
        var board = await _kanbanService.GetBoardAsync(boardId);
        if (!board.Success)
        {
            _logger.LogWarning($"Failed to retrieve board with ID '{boardId}': {board.ErrorMessage}");
            return BadRequest(board.ErrorMessage);
        }

        _logger.LogInformation($"Board with ID '{boardId}' retrieved successfully.");

        return Ok(board.Data);
    }

    [HttpPut]
    public async Task<IActionResult> PutBoard([FromBody] Board board)
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

        _logger.LogError($"Failed to create or update the board: {board.BoardId}");
        return StatusCode(500, new
        {
            message = "An error occurred while processing your request."
        });
    }

    [HttpPut("{boardId}/columns")]
    public async Task<IActionResult> PutColumn(string boardId, [FromBody] Column column)
    {
        var result = await _kanbanService.PutColumnAsync(boardId, column);
        if (!result.Success)
        {
            _logger.LogWarning($"Failed to add column to board with ID '{boardId}': {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }

        _logger.LogInformation($"Column added to board with ID '{boardId}'");
        return Ok(new
        {
            message = "Column added", board = result.Data
        });
    }

    [HttpPut("{boardId}/columns/{columnTitle}")]
    public async Task<IActionResult> PutColumn(string boardId, string columnTitle)
    {
        var column = new Column
        {
            ColumnId = Guid.NewGuid(), Title = columnTitle, Tickets = new List<Ticket>()
        };
        var result = await _kanbanService.PutColumnAsync(boardId, column);
        if (!result.Success)
        {
            _logger.LogWarning($"Failed to add column to board with ID '{boardId}': {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }

        _logger.LogInformation($"Column added to board with ID '{boardId}'");
        return Ok(new
        {
            message = "Column added", board = result.Data
        });
    }

    [HttpDelete("{boardId}/columns/{columnId}")]
    public async Task<IActionResult> RemoveColumn(string boardId, Guid columnId)
    {
        var result = await _kanbanService.RemoveColumnAsync(boardId, columnId);
        if (!result.Success)
        {
            _logger.LogWarning(
                $"Failed to remove column with ID '{columnId}' from board with ID '{boardId}': {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }

        _logger.LogInformation($"Column with ID '{columnId}' removed from board with ID '{boardId}'");
        return Ok(new
        {
            message = "Column removed", board = result.Data
        });
    }

    [HttpPut("{boardId}/columns/{columnId}/tickets")]
    public async Task<IActionResult> PutTicket(string boardId, Guid columnId, [FromBody] Ticket ticket)
    {
        var result = await _kanbanService.PutTicketAsync(boardId, columnId, ticket);
        if (!result.Success)
        {
            _logger.LogWarning(
                $"Failed to add ticket to column with ID '{columnId}' on board with ID '{boardId}': {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }

        _logger.LogInformation($"Ticket added to column with ID '{columnId}' on board with ID '{boardId}'");
        return Ok(new
        {
            message = "Ticket added", board = result.Data
        });
    }

    [HttpDelete("{boardId}/columns/{columnId}/tickets/{ticketId}")]
    public async Task<IActionResult> RemoveTicket(string boardId, Guid columnId, Guid ticketId)
    {
        var result = await _kanbanService.RemoveTicketAsync(boardId, columnId, ticketId);
        if (!result.Success)
        {
            _logger.LogWarning(
                $"Failed to remove ticket with ID '{ticketId}' from column with ID '{columnId}' on board with ID '{boardId}': {result.ErrorMessage}");
            return BadRequest(result.ErrorMessage);
        }

        _logger.LogInformation(
            $"Ticket with ID '{ticketId}' removed from column with ID '{columnId}' on board with ID '{boardId}'");
        return Ok(new
        {
            message = "Ticket removed", board = result.Data
        });
    }
}