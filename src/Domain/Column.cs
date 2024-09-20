namespace Domain;

public record Column
{
    public required Guid ColumnId { get; init; }
    public List<Column>? Columns;
    public required string Title { get; set; }
    public required List<Ticket> Tickets { get; set; }
}