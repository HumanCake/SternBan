using Domain;

namespace ApplicationTests;

public class BoardValidatorTests
{
    private IBoardValidator _boardValidator;

    [SetUp]
    public void SetUp()
    {
        _boardValidator = new BoardValidator();
    }

    [Test]
    public void Validate_ShouldReturnValidResult_WhenBoardIsValid()
    {
        // Arrange
        var board = new Board
        {
            BoardId = "b1",
            Title = "Valid Board",
            Columns = new List<Column>
            {
                new()
                {
                    Title = "Column 1",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "t1", description = "Ticket 1 Description" }
                    }
                }
            }
        };

        // Act
        var result = _boardValidator.Validate(board);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public void Validate_ShouldReturnInvalidResult_WhenBoardIdIsNullOrEmpty()
    {
        // Arrange
        var board = new Board
        {
            BoardId = "",
            Title = "Valid Board",
            Columns = new List<Column>
            {
                new()
                {
                    Title = "Column 1",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "t1", description = "Ticket 1 Description" }
                    }
                }
            }
        };

        // Act
        var result = _boardValidator.Validate(board);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Contains("BoardId cannot be null or empty"));
    }

    [Test]
    public void Validate_ShouldReturnInvalidResult_WhenTitleIsNullOrEmpty()
    {
        // Arrange
        var board = new Board
        {
            BoardId = "b1",
            Title = "",
            Columns = new List<Column>
            {
                new()
                {
                    Title = "Column 1",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "t1", description = "Ticket 1 Description" }
                    }
                }
            }
        };

        // Act
        var result = _boardValidator.Validate(board);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Contains("Title cannot be null or empty"));
    }

    [Test]
    public void Validate_ShouldReturnInvalidResult_WhenColumnsAreNullOrEmpty()
    {
        // Arrange
        var board = new Board
        {
            BoardId = "b1",
            Title = "Valid Board",
            Columns = null
        };

        // Act
        var result = _boardValidator.Validate(board);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Contains("Columns cannot be null or empty"));
    }

    [Test]
    public void Validate_ShouldReturnInvalidResult_WhenColumnTitleIsNullOrEmpty()
    {
        // Arrange
        var board = new Board
        {
            BoardId = "b1",
            Title = "Valid Board",
            Columns = new List<Column>
            {
                new()
                {
                    Title = "",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "t1", description = "Ticket 1 Description" }
                    }
                }
            }
        };

        // Act
        var result = _boardValidator.Validate(board);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Contains("Column Title cannot be null or empty"));
    }

    [Test]
    public void Validate_ShouldReturnInvalidResult_WhenTicketNameIsNullOrEmpty()
    {
        // Arrange
        var board = new Board
        {
            BoardId = "b1",
            Title = "Valid Board",
            Columns = new List<Column>
            {
                new()
                {
                    Title = "Column 1",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "", description = "Ticket 1 Description" }
                    }
                }
            }
        };

        // Act
        var result = _boardValidator.Validate(board);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Contains("Ticket name cannot be null or empty"));
    }

    [Test]
    public void Validate_ShouldReturnInvalidResult_WhenTicketDescriptionIsNullOrEmpty()
    {
        // Arrange
        var board = new Board
        {
            BoardId = "b1",
            Title = "Valid Board",
            Columns = new List<Column>
            {
                new()
                {
                    Title = "Column 1",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "t1", description = "" }
                    }
                }
            }
        };

        // Act
        var result = _boardValidator.Validate(board);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Contains("Ticket description cannot be null or empty"));
    }
}