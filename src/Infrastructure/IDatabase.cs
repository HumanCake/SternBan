using Domain;

namespace Infrastructure;

public interface IDatabase
{
    Task<Board?> GetBoardAsync(string boardId);

    Task<Board> PutBoardAsync(Board board);
}