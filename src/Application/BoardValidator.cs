using Domain;

public interface IBoardValidator
{
    ValidationResult Validate(Board board);
}

public class BoardValidator : IBoardValidator
{
    public ValidationResult Validate(Board board)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrEmpty(board.BoardId))
        {
            result.IsValid = false;
            result.Errors.Add("BoardId cannot be null or empty");
        }

        if (string.IsNullOrEmpty(board.Title))
        {
            result.IsValid = false;
            result.Errors.Add("Title cannot be null or empty");
        }

        if (board.Columns == null || board.Columns.Count == 0)
        {
            result.IsValid = false;
            result.Errors.Add("Columns cannot be null or empty");
        }
        else
        {
            foreach (var column in board.Columns)
            {
                if (string.IsNullOrEmpty(column.Title))
                {
                    result.IsValid = false;
                    result.Errors.Add("Column Title cannot be null or empty");
                }
                else
                {
                    foreach (var ticket in column.Tickets)
                    {
                        if (string.IsNullOrEmpty(ticket.name))
                        {
                            result.IsValid = false;
                            result.Errors.Add("Ticket name cannot be null or empty");
                        }

                        if (string.IsNullOrEmpty(ticket.description))
                        {
                            result.IsValid = false;
                            result.Errors.Add("Ticket description cannot be null or empty");
                        }
                    }
                }
            }
        }

        return result;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}