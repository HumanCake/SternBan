namespace Domain;

public record Column
{
    public List<Column>? Columns;
    public string Title { get; set; }
    public List<Ticket> Tickets { get; set; }
}