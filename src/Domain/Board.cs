namespace Domain;

public record Board
{
    public string BoardId { get; init; }
    public List<Column> Columns { get; set; }
}