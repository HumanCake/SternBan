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
        //Arrange
        var board = new Board
        {
            BoardId = null!, Title = null!, Columns = null!
        };
        var expectedBoard = OperationResult<Board>.ErrorResult("Invalid");

        _kanbanService.PutBoardAsync(board)
            .Returns(expectedBoard);

        //Act
        var result = await _kanbanController.PutBoard(board);

        //Assert
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.That(badRequestObjectResult, Is.Not.Null);
        Assert.That(badRequestObjectResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestObjectResult.Value, Is.Not.Null);
    }

    [Test]
    public async Task PutColumnAsync_InvalidBoard_ReturnsBadRequest()
    {
        //Arrange
        var existingBoard = Board.DefaultBoard();
        var columnToPut = new Column
        {
            ColumnId = Guid.NewGuid(), Title = null!
        };
        var boardToValidate = Board.DefaultBoard();
        boardToValidate.Columns.Add(columnToPut);
        var validationResult = await _boardValidator.ValidateAsync(boardToValidate);
        var kanbanServicePutColumnAsyncResult = OperationResult<Board>.ErrorResult(validationResult.ToString());

        _kanbanService.PutColumnAsync(existingBoard.BoardId, columnToPut)
            .Returns(kanbanServicePutColumnAsyncResult);

        //Act
        var result = await _kanbanController.PutColumn(existingBoard.BoardId, columnToPut);

        //Assert
        var badRequestObject = result as BadRequestObjectResult;
        Assert.That(badRequestObject, Is.Not.Null);
        Assert.That(badRequestObject.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(badRequestObject.Value!.ToString()!.Contains("not be empty"));
        _logger.Received(1)
            .LogWarning(
                $"Failed to add column to board with ID '{existingBoard.BoardId}': {kanbanServicePutColumnAsyncResult.ErrorMessage}");
    }

    [Test]
    public async Task PutColumnAsync_ValidOperation_ReturnsOkResult()
    {
        //Arrange
        var existingBoard = Board.DefaultBoard();
        var columnToPut = existingBoard.Columns.FirstOrDefault()! with
        {
            Title = "new"
        };
        var expectedBoard = Board.DefaultBoard();
        expectedBoard.Columns.Add(columnToPut);
        _kanbanService.PutColumnAsync(existingBoard.BoardId, columnToPut)
            .Returns(OperationResult<Board>.SuccessResult(expectedBoard));

        //Act
        var result = await _kanbanController.PutColumn(existingBoard.BoardId, columnToPut);

        //Assert
        var resultAsOkResultObject = result as OkObjectResult;
        Assert.That(resultAsOkResultObject, Is.Not.Null);
        Assert.That(resultAsOkResultObject.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(resultAsOkResultObject.Value!.ToString(), Is.EqualTo(new
        {
            message = "Column added", board = expectedBoard
        }.ToString()));
        _logger.Received(1).LogInformation($"Column added to board with ID '{existingBoard.BoardId}'");
    }

    [Test]
    public async Task RemoveColumn_KanbanServiceIsUnsuccessfull_ReturnsBadRequest()
    {
        //Arrange
        var boardId = "boardId";
        var columnId = Guid.NewGuid();
        var errorMessage = "Something went wrong";
        var errorResult = OperationResult<Board>.ErrorResult(errorMessage);
        _kanbanService.RemoveColumnAsync(boardId, columnId)
            .Returns(errorResult);

        //Act
        var result = await _kanbanController.RemoveColumn(boardId, columnId);

        //Assert
        _logger.Received(1)
            .LogWarning(
                $"Failed to remove column with ID '{columnId}' from board with ID '{boardId}': {errorResult.ErrorMessage}");
        var resultAsOkResultObject = result as BadRequestObjectResult;
        Assert.That(resultAsOkResultObject, Is.Not.Null);
        Assert.That(resultAsOkResultObject.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public async Task RemoveColumn_Success_ReturnsOkResult()
    {
        //Arrange
        var boardId = Board.DefaultBoard().BoardId;
        var columnId = Board.DefaultBoard().Columns.FirstOrDefault()!.ColumnId;
        var successResult = OperationResult<Board>.SuccessResult(Board.DefaultBoard());
        _kanbanService.RemoveColumnAsync(boardId, columnId)
            .Returns(successResult);

        //Act
        var result = await _kanbanController.RemoveColumn(boardId, columnId);

        //Assert
        _logger.Received(1).LogInformation($"Column with ID '{columnId}' removed from board with ID '{boardId}'");
        var resultAsOkResultObject = result as OkObjectResult;
        Assert.That(resultAsOkResultObject, Is.Not.Null);
        Assert.That(resultAsOkResultObject.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(resultAsOkResultObject.Value!.ToString(), Is.EqualTo(new
        {
            message = "Column removed", board = Board.DefaultBoard()
        }.ToString()));
    }

    [Test]
    public async Task PutTicket_KanbanServiceIsUnSucessfull_ReturnBadRequest()
    {
        //Arrange
        var boardId = "boardId";
        var columnId = Guid.NewGuid();
        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid(), Title = "title"
        };
        var errorMessage = "Something went wrong";
        var errorResult = OperationResult<Board>.ErrorResult(errorMessage);
        _kanbanService.PutTicketAsync(boardId, columnId, ticket)
            .Returns(errorResult);

        //Act
        var result = await _kanbanController.PutTicket(boardId, columnId, ticket);

        //Assert
        _logger.Received(1)
            .LogWarning(
                $"Failed to add ticket to column with ID '{columnId}' on board with ID '{boardId}': {errorMessage}");
        var resultAsOkResultObject = result as BadRequestObjectResult;
        Assert.That(resultAsOkResultObject, Is.Not.Null);
        Assert.That(resultAsOkResultObject.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public async Task PutTicket_Success_ReturnOkResult()
    {
        //Arrange
        var boardId = Board.DefaultBoard().BoardId;
        var columnId = Board.DefaultBoard().Columns.FirstOrDefault()!.ColumnId;
        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid(), Title = "some title"
        };
        var successResult = OperationResult<Board>.SuccessResult(Board.DefaultBoard());
        _kanbanService.PutTicketAsync(boardId, columnId, ticket)
            .Returns(successResult);

        //Act
        var result = await _kanbanController.PutTicket(boardId, columnId, ticket);

        //Assert
        _logger.Received(1).LogInformation($"Ticket added to column with ID '{columnId}' on board with ID '{boardId}'");
        var resultAsOkResultObject = result as OkObjectResult;
        Assert.That(resultAsOkResultObject, Is.Not.Null);
        Assert.That(resultAsOkResultObject.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(resultAsOkResultObject.Value!.ToString(), Is.EqualTo(new
        {
            message = "Ticket added", board = Board.DefaultBoard()
        }.ToString()));
    }
}