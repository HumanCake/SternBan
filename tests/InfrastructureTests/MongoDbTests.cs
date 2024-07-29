using Domain;
using Infrastructure;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace InfrastructureTests;

public class MongoDbTests
{
    private readonly Board _defaultBoard = Board.DefaultBoard();
    private IMongoClient _mongoClient;
    private MongoDb _mongoDb;
    private MongoDbContainer _mongoDbContainer;

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
        await collection.InsertOneAsync(_defaultBoard);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _mongoDbContainer.StopAsync();
    }

    [Test]
    public async Task GetBoardAsync_BoardExists_ReturnsBoard()
    {
        // Arrange
        var boardId = _defaultBoard.BoardId;

        // Act
        var result = await _mongoDb.GetBoardAsync(boardId);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.BoardId, Is.EqualTo(boardId));
    }

    [Test]
    public async Task GetBoardAsync_BoardDoesNotExist_ReturnsNull()
    {
        // Arrange
        var boardId = "non_existent_board_id";

        // Act
        var result = await _mongoDb.GetBoardAsync(boardId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task PutBoardAsync_BoardDoesNotExist_InsertsBoard()
    {
        // Arrange
        var board = _defaultBoard with
        {
            BoardId = "new id"
        };

        // Act
        await _mongoDb.PutBoardAsync(board);

        // Assert
        var actualBoard = await _mongoDb.GetBoardAsync(board.BoardId);
        Assert.NotNull(actualBoard);
        Assert.That(actualBoard.BoardId, Is.EqualTo(board.BoardId));
    }

    [Test]
    public async Task PutBoardAsync_BoardExists_UpdatesBoard()
    {
        // Arrange
        var board = _defaultBoard with
        {
            Title = "Updated Board"
        };

        // Act
        await _mongoDb.PutBoardAsync(board);

        // Assert
        var actualBoard = await _mongoDb.GetBoardAsync(board.BoardId);
        Assert.NotNull(actualBoard);
        Assert.That(actualBoard.BoardId, Is.EqualTo(board.BoardId));
        Assert.That(actualBoard.Title, Is.EqualTo(board.Title));
    }
}