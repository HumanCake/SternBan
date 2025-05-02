using Domain;

namespace Infrastructure;

public interface IDatabase
{
    Task<List<Board>> GetBoardsAsync();
    Task<Board?> GetBoardAsync(string boardId);

    Task<Board> PutBoardAsync(Board board);
}