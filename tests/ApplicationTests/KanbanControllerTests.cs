using Application;
using Domain;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ApplicationTests;

public class KanbanControllerTests
{
    private IValidator<Board> _boardValidator;
    private KanbanController _kanbanController;
    private IKanbanService _kanbanService;
    private ILogger<KanbanController> _logger;

    [SetUp]
    public void SetUp()
    {
        _kanbanService = Substitute.For<IKanbanService>();
        _logger = Substitute.For<ILogger<KanbanController>>();
        _boardValidator = new BoardValidator();
        _kanbanController = new KanbanController(_logger, _kanbanService, _boardValidator);
    }

    [Test]
    public async Task GetBoard_BoardFound_ShouldReturnOk()
    {
        // Arrange
        var boardId = "123";
        var board = Board.DefaultBoard();
        var expectedBoard = OperationResult<Board>.SuccessResult(board);

        _kanbanService.GetBoardAsync(boardId)
            .Returns(expectedBoard);

        // Act
        var result = await _kanbanController.GetBoard(boardId);

        // Assert
        var okObjectResult = result as OkObjectResult;
        Assert.That(okObjectResult, Is.Not.Null);
        Assert.That(okObjectResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(okObjectResult.Value, Is.EqualTo(expectedBoard.Data));
    }

    [Test]
    public async Task GetBoard_BoardFound_ShouldLogSuccessMessage()
    {
        // Arrange
        var boardId = "123";
        var board = Board.DefaultBoard();
        _kanbanService.GetBoardAsync(boardId)
            .Returns(OperationResult<Board>.SuccessResult(board));

        // Act
        await _kanbanController.GetBoard(boardId);

        // Assert
        _logger.Received(1).LogInformation($"Board with ID '{boardId}' retrieved successfully.");
    }

    [Test]
    public async Task PutBoard_ValidBoard_ShouldReturnOkWithCorrectMessage()
    {
        // Arrange
        var board = Board.DefaultBoard();
        var expectedBoard = OperationResult<Board>.SuccessResult(board);

        _kanbanService.PutBoardAsync(board)
            .Returns(expectedBoard);

        // Act
        var result = await _kanbanController.PutBoard(board);

        // Assert
        var okObjectResult = result as OkObjectResult;
        Assert.That(okObjectResult, Is.Not.Null);
        Assert.That(okObjectResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(okObjectResult.Value, Is.EqualTo("The board was created or updated:\n" + expectedBoard.Data));
    }

    [Test]
    public async Task PutBoard_InvalidBoard_ShouldReturnBadRequest()
    {
        // Arrange
        var board = new Board
        {
            BoardId = null!, Title = null!, Columns = null!
        };
        var expectedBoard = OperationResult<Board>.ErrorResult("Invalid");

        _kanbanService.PutBoardAsync(board)
            .Returns(expectedBoard);

        // Act
        var result = await _kanbanController.PutBoard(board);

        // Assert
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.That(badRequestObjectResult, Is.Not.Null);
        Assert.That(badRequestObjectResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestObjectResult.Value, Is.Not.Null);
    }
}