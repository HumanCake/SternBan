namespace Domain;

public record Ticket
{
    public required Guid TicketId { get; init; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}