using Domain;
using Infrastructure;
using MongoDB.Driver;
using Testcontainers.MongoDb;

public class MongoDbTests
{
    public Board DefaultBoard = Board.DefaultBoard();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .Build();

        await _mongoDbContainer.StartAsync();
        _mongoClient = new MongoClient(_mongoDbContainer.GetConnectionString());
        _mongoDb = new MongoDb(_mongoClient);

        var database = _mongoClient.GetDatabase("your_database_name");
        var collection = database.GetCollection<Board>("boards");
        await collection.InsertOneAsync(DefaultBoard);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _mongoDbContainer.StopAsync();
    }

    private MongoDbContainer _mongoDbContainer;
    private IMongoClient _mongoClient;
    private MongoDb _mongoDb;

    [Test]
    public async Task GetBoardAsync_ReturnsBoard_WhenBoardExists()
    {
        // Arrange
        var boardId = DefaultBoard.BoardId;

        // Act
        var result = await _mongoDb.GetBoardAsync(boardId);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(boardId, result.BoardId);
    }

    [Test]
    public async Task GetBoardAsync_ReturnsNull_WhenBoardDoesNotExist()
    {
        // Arrange
        var boardId = "non_existent_board_id";

        // Act
        var result = await _mongoDb.GetBoardAsync(boardId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task PutBoardAsync_InsertsBoard_WhenBoardDoesNotExist()
    {
        // Arrange
        var board = DefaultBoard with { BoardId = "new id" };

        // Act
        await _mongoDb.PutBoardAsync(board);

        // Assert
        var actualBoard = await _mongoDb.GetBoardAsync(board.BoardId);
        Assert.NotNull(actualBoard);
        Assert.AreEqual(board.BoardId, actualBoard.BoardId);
    }

    [Test]
    public async Task PutBoardAsync_UpdatesBoard_WhenBoardExists()
    {
        // Arrange
        var board = DefaultBoard with { Title= "Updated Board" };

        // Act
        await _mongoDb.PutBoardAsync(board);

        // Assert
        var actualBoard = await _mongoDb.GetBoardAsync(board.BoardId);
        Assert.NotNull(actualBoard);
        Assert.AreEqual(board.BoardId, actualBoard.BoardId);
        Assert.AreEqual(board.Title, actualBoard.Title);
    }
}