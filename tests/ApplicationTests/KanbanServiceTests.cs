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
        Assert.That(result.ErrorMessage.Contains("not found"));
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
        Assert.That(result.Data.Columns.Any(column => column.Title == columnToPut.Title), Is.True);
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
        Assert.That(result.ErrorMessage.Contains("Board not found"));
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
        Assert.That(result.ErrorMessage.Contains("must not be empty"));
    }

    [Test]
    public async Task RemoveColumnAsync_ColumnExists_ShouldReturnSuccessResult()
    {
        //Arrange
        var columnToRemove = _defaultBoard.Columns.FirstOrDefault();
        var boardId = _defaultBoard.BoardId;
        _database.GetBoardAsync(boardId)
            .Returns(_defaultBoard);
        _database.PutBoardAsync(Arg.Any<Board>())
            .Returns(x => x.Arg<Board>());

        //Act
        var result = await _kanbanService.RemoveColumnAsync(boardId, columnToRemove.Title);

        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.Columns.Any(c => c.Title == columnToRemove.Title), Is.False);
    }

    [Test]
    public async Task RemoveColumnAsync_RemovingLastColumn_ShouldReturnErrorResult()
    {
        //Arrange
        var columnToRemove = _defaultBoard.Columns.FirstOrDefault();
        var board = _defaultBoard with
        {
            Columns = new List<Column>
            {
                columnToRemove
            }
        };
        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(board);

        //Act
        var result = await _kanbanService.RemoveColumnAsync(board.BoardId, columnToRemove.Title);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("must not be empty"));
    }

    [Test]
    public async Task RemoveColumnAsync_ColumnNotFound_ShouldReturnErrorResult()
    {
        //Arrange
        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(_defaultBoard);

        //Act
        var result = await _kanbanService.RemoveColumnAsync(_defaultBoard.BoardId, "unknown");

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("Column not found"));
    }

    [Test]
    public async Task RemoveColumnAsync_BordNotFound_ShouldReturnErrorResult()
    {
        //Arrange
        _database.GetBoardAsync(Arg.Any<string>())
            .Returns(Task.FromResult<Board>(null));

        //Act
        var result = await _kanbanService.RemoveColumnAsync("unknown", "unknownh");

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("Board not found"));
    }

    [Test]
    public async Task PutTicketAsync_BoardAndColumnFound_ReturnSuccessResult()
    {
        //Arrange
        var ticketName = "new ticket";
        var ticketToPut = _defaultBoard.Columns.FirstOrDefault().Tickets.FirstOrDefault() with
        {
            Title = ticketName
        };

        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(_defaultBoard);
        _database.PutBoardAsync(Arg.Any<Board>())
            .Returns(x => x.Arg<Board>());

        //Act
        var result = await _kanbanService.PutTicketAsync(_defaultBoard.BoardId,
            _defaultBoard.Columns.FirstOrDefault().Title, ticketToPut);

        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.Columns.FirstOrDefault().Tickets.Any(ticket => ticket.Title == ticketName), Is.True);
    }

    [Test]
    public async Task PutTicketAsync_ColumnNotFound_ShouldReturnErrorResult()
    {
        //Arrange
        var ticketName = "new ticket";
        var ticketToPut = _defaultBoard.Columns.FirstOrDefault().Tickets.FirstOrDefault() with
        {
            Title = ticketName
        };

        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(_defaultBoard);

        //Act
        var result = await _kanbanService.PutTicketAsync(_defaultBoard.BoardId,
            "unknown column", ticketToPut);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("Column not found"));
    }

    [Test]
    public async Task PutTicketAsync_BoardNotFound_ShouldReturnErrorResult()
    {
        //Arrange
        var ticketName = "new ticket";
        var ticketToPut = _defaultBoard.Columns.FirstOrDefault().Tickets.FirstOrDefault() with
        {
            Title = ticketName
        };

        _database.GetBoardAsync(Arg.Any<string>())
            .Returns(Task.FromResult<Board>(null));

        //Act
        var result = await _kanbanService.PutTicketAsync(_defaultBoard.BoardId,
            _defaultBoard.Columns.FirstOrDefault().Title, ticketToPut);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("Board not found"));
    }

    [Test]
    public async Task PutTicketAsync_InvalidTicket_ReturnsErrorResult()
    {
        //Arrange
        var ticketToPut = _defaultBoard.Columns.FirstOrDefault().Tickets.FirstOrDefault() with
        {
            Title = ""
        };

        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(_defaultBoard);

        //Act
        var result = await _kanbanService.PutTicketAsync(_defaultBoard.BoardId,
            _defaultBoard.Columns.FirstOrDefault().Title, ticketToPut);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("must not be empty"));
    }

    [Test]
    public async Task RemoveTicketAsync_TicketExists_ReturnSuccessResult()
    {
        //Arrange
        var ticketToRemove = _defaultBoard.Columns.FirstOrDefault().Tickets.FirstOrDefault();
        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(_defaultBoard);
        _database.PutBoardAsync(Arg.Any<Board>())
            .Returns(x => x.Arg<Board>());

        //Act
        var result = await _kanbanService.RemoveTicketAsync(_defaultBoard.BoardId,
            _defaultBoard.Columns.FirstOrDefault().Title, ticketToRemove.Title);

        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data.Columns.FirstOrDefault().Tickets.Any(ticket => ticket.Title == ticketToRemove.Title),
            Is.False);
    }

    [Test]
    public async Task RemoveTicketAsync_TicketDoesNotExist_ReturnErrorResult()
    {
        //Arrange
        var ticketToRemove = "unkown ticket";
        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(_defaultBoard);

        //Act
        var result = await _kanbanService.RemoveTicketAsync(_defaultBoard.BoardId,
            _defaultBoard.Columns.FirstOrDefault().Title, ticketToRemove);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("Ticket not found"));
    }

    [Test]
    public async Task RemoveTicketAsync_ColumnDoesNotExist_ReturnErrorResult()
    {
        //Arrange
        var ticketToRemove = _defaultBoard.Columns.FirstOrDefault().Tickets.FirstOrDefault();
        _database.GetBoardAsync(_defaultBoard.BoardId)
            .Returns(_defaultBoard);

        //Act
        var result = await _kanbanService.RemoveTicketAsync(_defaultBoard.BoardId,
            "unknown column", ticketToRemove.Title);

        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage.Contains("Column not found"));
    }
}