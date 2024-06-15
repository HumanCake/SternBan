namespace Domain;

public record Ticket
{
    public string? Description { get; set; }
    public required string Title { get; set; }
}