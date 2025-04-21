using System.Diagnostics;
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

    public async Task<OperationResult<List<Board>>> GetBoardsAsync()
    {
        var boards = await _database.GetBoardsAsync();
        if(!boards.Any())
            return OperationResult<List<Board>>.ErrorResult("No boards found");

        return OperationResult<List<Board>>.SuccessResult(boards);
    }

    public async Task<OperationResult<Board>> GetBoardAsync(string boardId)
    {
        var board = await _database.GetBoardAsync(boardId);
        if (board == null) return OperationResult<Board>.ErrorResult("Board not found");

        return OperationResult<Board>.SuccessResult(board);
    }

    public async Task<OperationResult<Board>> PutBoardAsync(Board board)
    {
        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid)
        {
            return OperationResult<Board>.ErrorResult(validationResult.ToString() ?? string.Empty);
        }

        board = await _database.PutBoardAsync(board);
        return OperationResult<Board>.SuccessResult(board);
    }

    public async Task<OperationResult<Board>> PutColumnAsync(string boardId, Column column)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success) return boardResult;

        var board = boardResult.Data;
        board?.Columns.Add(column);

        var validationResult = await _boardValidator.ValidateAsync(board ?? throw new InvalidOperationException());
        if (!validationResult.IsValid) return OperationResult<Board>.ErrorResult(validationResult.ToString());

        return await PutBoardAsync(board);
    }

    public async Task<OperationResult<Board>> RemoveColumnAsync(string boardId, Guid columnId)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success) return boardResult;

        var board = boardResult.Data;
        Debug.Assert(board != null, nameof(board) + " != null");
        var column = board.Columns.FirstOrDefault(c => c.ColumnId == columnId);
        if (column == null) return OperationResult<Board>.ErrorResult("Column not found");

        board.Columns.Remove(column);

        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid) return OperationResult<Board>.ErrorResult(validationResult.ToString());
        return await PutBoardAsync(board);
    }

    public async Task<OperationResult<Board>> PutTicketAsync(string boardId, Guid columnId, Ticket ticket)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success) return boardResult;

        var board = boardResult.Data;

        Debug.Assert(board != null, nameof(board) + " != null");
        var column = board.Columns.FirstOrDefault(c => c.ColumnId == columnId);
        if (column == null) return OperationResult<Board>.ErrorResult("Column not found");

        column.Tickets?.Add(ticket);
        var validationResult = await _boardValidator.ValidateAsync(board);

        if (!validationResult.IsValid)
        {
            return OperationResult<Board>.ErrorResult(validationResult.ToString());
        }

        return await PutBoardAsync(board);
    }

    public async Task<OperationResult<Board>> RemoveTicketAsync(string boardId, Guid columnId, Guid ticketId)
    {
        var boardResult = await GetBoardAsync(boardId);
        if (!boardResult.Success) return boardResult;

        var board = boardResult.Data;

        Debug.Assert(board != null, nameof(board) + " != null");
        var column = board.Columns.FirstOrDefault(c => c.ColumnId == columnId);
        if (column == null) return OperationResult<Board>.ErrorResult("Column not found");
        var ticket = column.Tickets?.FirstOrDefault(t => t.TicketId == ticketId);
        if (ticket == null) return OperationResult<Board>.ErrorResult("Ticket not found");

        column.Tickets?.Remove(ticket);
        var validationResult = await _boardValidator.ValidateAsync(board);
        if (!validationResult.IsValid) return OperationResult<Board>.ErrorResult(validationResult.ToString());
        return await PutBoardAsync(board);
    }
}