using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Application;

[Route("api/[controller]")]
[ApiController]
public class KanbanController : ControllerBase
{
    private readonly ILogger<KanbanController> _logger;

    public KanbanController(ILogger<KanbanController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetBoard(string boardId)
    {
        // Example: Retrieve board from the database based on boardId
        var board = RetrieveBoardFromDatabase(boardId);

        if (board == null)
        {
            _logger.LogWarning($"Board with ID '{boardId}' not found.");
            return NotFound();
        }

        _logger.LogInformation($"Board with ID '{boardId}' retrieved successfully.");

        return Ok(board);
    }

    // Dummy method to simulate retrieving a board from the database
    private Board RetrieveBoardFromDatabase(string boardId)
    {
        // Example: This could be replaced with actual database retrieval logic
        // For demonstration, creating a dummy board with sample data
        var columns = new List<Column>
        {
            new() { Title = "1", Tickets = new List<Ticket>() },
            new() { Title = "2", Tickets = new List<Ticket>() },
            new() { Title = "3", Tickets = new List<Ticket>() }
        };

        return new Board { BoardId = boardId, Columns = columns };
    }
}