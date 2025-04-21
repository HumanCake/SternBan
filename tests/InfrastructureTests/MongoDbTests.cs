using System.Diagnostics;
using AutoFixture;
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
    private Fixture _fixture;
    private IMongoCollection<Board>? _collection;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .Build();

        await _mongoDbContainer.StartAsync();
        _mongoClient = new MongoClient(_mongoDbContainer.GetConnectionString());
        _mongoDb = new MongoDb(_mongoClient);

        var database = _mongoClient.GetDatabase("your_database_name");
        _collection = database.GetCollection<Board>("boards");
        await _collection.InsertOneAsync(_defaultBoard);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _mongoDbContainer.StopAsync();
        await _mongoDbContainer.DisposeAsync();
    }

    [Test]
    public async Task GetBoardsAsync_BoardsExits_ReturnsBoards()
    {
        // Arrange
        var boards = _fixture.Create<List<Board>>();
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        await _collection.InsertManyAsync(boards);
        
        // Act
        var result = await _mongoDb.GetBoardsAsync();
        
        // Assert
        Assert.That(result, Has.Count.GreaterThanOrEqualTo(boards.Count));
        foreach (var board in boards)
        {
            Assert.That(result.Any(r => r.BoardId == board.BoardId), Is.True, $"Board with ID {board.BoardId} was not found in the result.");
        }
    }

    [Test]
    public async Task GetBoardAsync_BoardExists_ReturnsBoard()
    {
        // Arrange
        var boardId = _defaultBoard.BoardId;

        // Act
        var result = await _mongoDb.GetBoardAsync(boardId);

        // Assert
        Assert.That(result, Is.Not.Null);
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
        Assert.That(result, Is.Null);
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
        Assert.That(actualBoard, Is.Not.Null);
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
        Assert.That(actualBoard, Is.Not.Null);
        Assert.That(actualBoard.BoardId, Is.EqualTo(board.BoardId));
        Assert.That(actualBoard.Title, Is.EqualTo(board.Title));
    }
}