using Domain;
using FluentValidation;
using Infrastructure;

namespace Application;

public class KanbanService : IKanbanService
{
    private readonly IDatabase _database;
    private readonly IValidator<Board> _boardValidator;

    public KanbanService(IDatabase database, IValidator<Board> boardValidator)
    {
        _database = database;
        _boardValidator = boardValidator;
    }

    public async Task<OperationResult<Board>> GetBoardAsync(string boardId)
    {
        var board = await _database.GetBoardAsync(boardId);
        if (board == null)
        {
            return OperationResult<Board>.ErrorResult("Board not found");
        }

        return OperationResult<Board>.SuccessResult(board);
    }

    public async Task<OperationResult<Board>> PutBoardAsync(Board board)
    {
        board = await _database.PutBoardAsync(board);
        return OperationResult<Board>.SuccessResult(board);
    }

    public async Task<OperationResult<Board>> PutColumnAsync(string boardId, Column column)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success)
        {
            return boardResult;
        }

        var board = boardResult.Data;
        board.Columns.Add(column);

        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid)
        {
            return OperationResult<Board>.ErrorResult(validationResult.Errors.ToString());
        }

        await _database.PutBoardAsync(board);
        return OperationResult<Board>.SuccessResult(board);
    }

    public async Task<OperationResult<Board>> RemoveColumnAsync(string boardId, string columnId)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success)
        {
            return boardResult;
        }

        var board = boardResult.Data;
        var column = board.Columns.FirstOrDefault(c => c.Title == columnId);
        if (column == null)
        {
            return OperationResult<Board>.ErrorResult("Column not found");
        }

        board.Columns.Remove(column);

        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid)
        {
            return OperationResult<Board>.ErrorResult(validationResult.Errors.ToString());
        }
        await _database.PutBoardAsync(board);
        return OperationResult<Board>.SuccessResult(board);
    }

    public Task<Board> PutTicket(string boardId, string columnId, Ticket ticket)
    {
        throw new NotImplementedException();
    }

    public Task<Board> RemoveTicket(string boardId, string columnId, string ticketId)
    {
        throw new NotImplementedException();
    }
}