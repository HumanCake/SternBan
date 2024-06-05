using Domain;

namespace Application;

public interface IKanbanService
{
    Task<Board> GetBoardAsync(string boardId);
    Task<Board> PutBoardAsync(Board board);
}