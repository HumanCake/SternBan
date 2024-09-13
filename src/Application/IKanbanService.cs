using Domain;

namespace Application;

public interface IKanbanService
{
    Task<OperationResult<Board>> GetBoardAsync(string boardId);
    Task<OperationResult<Board>> PutBoardAsync(Board board);
    Task<OperationResult<Board>> PutColumnAsync(string boardId, Column column);
    Task<OperationResult<Board>> RemoveColumnAsync(string boardId, Guid columnId);
    Task<OperationResult<Board>> PutTicketAsync(string boardId, Guid columnId, Ticket ticket);
    Task<OperationResult<Board>> RemoveTicketAsync(string boardId, Guid columnId, Guid ticketId);
}