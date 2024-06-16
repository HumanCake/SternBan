using Domain;
using FluentValidation;
using Infrastructure;

namespace Application;

public class KanbanService : IKanbanService
{
    private readonly IValidator<Board> _boardValidator;
    private readonly IDatabase _database;

    public KanbanService(IDatabase database, IValidator<Board> boardValidator)
    {
        _database = database;
        _boardValidator = boardValidator;
    }

    public async Task<OperationResult<Board>> GetBoardAsync(string boardId)
    {
        var board = await _database.GetBoardAsync(boardId);
        if (board == null) return OperationResult<Board>.ErrorResult("Board not found");

        return OperationResult<Board>.SuccessResult(board);
    }

    public async Task<OperationResult<Board>> PutBoardAsync(Board board)
    {
        var validationResult = _boardValidator.ValidateAsync(board);
        if (!validationResult.Result.IsValid) return OperationResult<Board>.ErrorResult(validationResult.ToString());

        board = await _database.PutBoardAsync(board);
        return OperationResult<Board>.SuccessResult(board);
    }

    public async Task<OperationResult<Board>> PutColumnAsync(string boardId, Column column)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success) return boardResult;

        var board = boardResult.Data;
        board.Columns.Add(column);

        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid) return OperationResult<Board>.ErrorResult(validationResult.ToString());

        return await PutBoardAsync(board);
    }

    public async Task<OperationResult<Board>> RemoveColumnAsync(string boardId, string columnId)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success) return boardResult;

        var board = boardResult.Data;
        var column = board.Columns.FirstOrDefault(c => c.Title == columnId);
        if (column == null) return OperationResult<Board>.ErrorResult("Column not found");

        board.Columns.Remove(column);

        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid) return OperationResult<Board>.ErrorResult(validationResult.ToString());
        return await PutBoardAsync(board);
    }

    public async Task<OperationResult<Board>> PutTicketAsync(string boardId, string columnId, Ticket ticket)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success) return boardResult;

        var board = boardResult.Data;

        var column = board.Columns.FirstOrDefault(c => c.Title == columnId);
        if (column == null) return OperationResult<Board>.ErrorResult("Column not found");

        column.Tickets.Add(ticket);
        var validationResult = await _boardValidator.ValidateAsync(board);

        if (!validationResult.IsValid)
        {
            return OperationResult<Board>.ErrorResult(validationResult.ToString());
        }

        return await PutBoardAsync(board);
    }

    public async Task<OperationResult<Board>> RemoveTicketAsync(string boardId, string columnId, string ticketId)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (boardResult.Success) return boardResult;

        var board = boardResult.Data;

        var column = board.Columns.FirstOrDefault(c => c.Title == columnId);
        if (column == null) return OperationResult<Board>.ErrorResult("Column not found");
        var ticket = column.Tickets.FirstOrDefault(t => t.Title == ticketId);
        if (ticket == null) return OperationResult<Board>.ErrorResult("Ticket not found");

        column.Tickets.Remove(ticket);
        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid) return OperationResult<Board>.ErrorResult(validationResult.ToString());
        return await PutBoardAsync(board);
    }
}