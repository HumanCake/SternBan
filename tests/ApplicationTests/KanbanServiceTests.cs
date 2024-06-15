using Application;
using Domain;
using FluentValidation;
using Infrastructure;
using NSubstitute;

namespace ApplicationTests;

public class KanbanServiceTests
{
    private readonly IValidator<Board> _boardValidator = new BoardValidator();
    private IDatabase _database;
    private Board _defaultBoard;
    private KanbanService _kanbanService;

    [SetUp]
    public void Setup()
    {
        _database = Substitute.For<IDatabase>();
        _kanbanService = new KanbanService(_database, _boardValidator);
        _defaultBoard = Board.DefaultBoard();
    }

    [Test]
    public async Task GetBoardAsync_BoardExists_ReturnsSuccessResult()
    {
        //Arrange
        var boardId = _defaultBoard.BoardId;
        _database.GetBoardAsync(boardId).Returns(Task.FromResult(_defaultBoard));

        //Act
        var result = await _kanbanService.GetBoardAsync(boardId);

        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.EqualTo(_defaultBoard));
    }

    [Test]
    public async Task GetBoardAsync_BoardDoesNotExist_ShouldReturnErrorResult()
    {
        //Arrange
        _database.GetBoardAsync(Arg.Any<string>())
            .Returns(Task.FromResult<Board>(null));

        //Act
        var result = await _kanbanService.GetBoardAsync("");

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage, Contains.Substring("not found"));
    }

    [Test]
    public async Task PutBoardAsync_Successful_ReturnsSuccessResult()
    {
        // Arrange
        _database.PutBoardAsync(_defaultBoard).Returns(Task.FromResult(_defaultBoard));

        // Act
        var result = await _kanbanService.PutBoardAsync(_defaultBoard);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.EqualTo(_defaultBoard));
    }

    [Test]
    public async Task PutBoardAsync_InvalidBoard_ReturnErrorResult()
    {
        //Arrange
        var invalidBoard = _defaultBoard with
        {
            Title = ""
        };

        //Act
        var result = await _kanbanService.PutBoardAsync(invalidBoard);

        //Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task PutColumnAsync_ValidColumnAndBoard_ReturnSuccessResult()
    {
        //Arrange
        var defaultBoard = _defaultBoard;
        var columnToPut = _defaultBoard.Columns.FirstOrDefault() with
        {
            Title = "new column"
        };
        var expectedBoard = defaultBoard;
        expectedBoard.Columns.Add(columnToPut);

        _database.GetBoardAsync(defaultBoard.BoardId)
            .Returns(defaultBoard);

        _database.PutBoardAsync(expectedBoard)
            .Returns(expectedBoard);

        //Act
        var result = await _kanbanService.PutColumnAsync(defaultBoard.BoardId, columnToPut);

        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.EqualTo(_defaultBoard));
    }

    [Test]
    public async Task PutColumnAsync_BoardDoesNotExist_ShouldReturnErrorResult()
    {
        //Arrange
        var columnToPut = _defaultBoard.Columns.FirstOrDefault() with
        {
            Title = "new column"
        };
        _database.GetBoardAsync(Arg.Any<string>())
            .Returns(Task.FromResult<Board>(null));

        //Act
        var result = await _kanbanService.PutColumnAsync(_defaultBoard.BoardId, columnToPut);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage, Contains.Substring("Board not found"));
    }

    [Test]
    public async Task PutColumnAsync_InvalidColumn_ReturnErrorResult()
    {
        //Arrange
        var invalidColumn = _defaultBoard.Columns.FirstOrDefault() with
        {
            Title = ""
        };
        _database.GetBoardAsync(Arg.Any<string>())
            .Returns(_defaultBoard);

        //Act
        var result = await _kanbanService.PutColumnAsync(_defaultBoard.BoardId, invalidColumn);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage, Contains.Substring("must not be empty"));
    }
}