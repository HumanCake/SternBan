using Domain;

namespace Application;

public interface IKanbanService
{
    Task<OperationResult<Board>> GetBoardAsync(string boardId);
    Task<OperationResult<Board>> PutBoardAsync(Board board);
    Task<OperationResult<Board>> PutColumnAsync(string boardId, Column column);
    Task<OperationResult<Board>> RemoveColumnAsync(string boardId, string columnId);
    Task<Board> PutTicket(string boardId, string columnId, Ticket ticket);
    Task<Board> RemoveTicket(string boardId, string columnId, string ticketId);

}