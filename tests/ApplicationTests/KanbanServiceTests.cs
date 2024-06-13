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
    private KanbanService _kanbanService;
    private Board _defaultBoard;

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
        Assert.That(result.ErrorMessage, Contains.Substring("Title"));
    }
}