using Domain;
using Infrastructure;

namespace Application;

public class KanbanService : IKanbanService
{
    private readonly IDatabase _database;

    public KanbanService(IDatabase database)
    {
        _database = database;
    }

    public async Task<Board> GetBoardAsync(string boardId)
    {
        var board = await _database.GetBoardAsync(boardId);
        return board;
    }

    public async Task<Board> PutBoardAsync(Board board)
    {
        board = await _database.PutBoardAsync(board);
        return board;
    }
}