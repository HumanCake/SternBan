namespace Domain;

public record Column
{
    public List<Column>? Columns;
    public required string Title { get; set; }
    public List<Ticket>? Tickets { get; set; }
}