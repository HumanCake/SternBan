using Domain;
using MongoDB.Driver;

namespace Infrastructure;

public class MongoDb : IDatabase
{
    private readonly IMongoCollection<Board> _boardsCollection;

    public MongoDb(IMongoClient mongoClient)
    {
        var database =
            mongoClient.GetDatabase("your_database_name");
        _boardsCollection = database.GetCollection<Board>("boards");
    }

    public async Task<Board?> GetBoardAsync(string boardId)
    {
        var filter = Builders<Board>.Filter.Eq(board => board.BoardId, boardId);

        var boardDocument = await _boardsCollection.Find(filter).FirstOrDefaultAsync();

        if (boardDocument == null) return null;

        return boardDocument;
    }

    public async Task<Board> PutBoardAsync(Board board)
    {
        var filter = Builders<Board>.Filter.Eq(b => b.BoardId, board.BoardId);

        await _boardsCollection.ReplaceOneAsync(filter, board, new ReplaceOptions
        {
            IsUpsert = true
        });

        return board;
    }
}