using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public record Board
{
    [BsonId] public required string BoardId { get; init; }

    public required string Title { get; set; }
    public required List<Column> Columns { get; set; }


    public static Board DefaultBoard()
    {
        return new Board
        {
            BoardId = "b1",
            Title = "Project Board",
            Columns = new List<Column>
            {
                new()
                {
                    ColumnId = Guid.NewGuid(),
                    Title = "To Do",
                    Tickets = new List<Ticket>
                    {
                        new()
                        {
                            TicketId = Guid.NewGuid(), Title = "t1", Description = "Ticket 1 Description"
                        },
                        new()
                        {
                            TicketId = Guid.NewGuid(), Title = "t2", Description = "Ticket 2 Description"
                        }
                    }
                },
                new()
                {
                    ColumnId = Guid.NewGuid(),
                    Title = "Doing",
                    Tickets = new List<Ticket>
                    {
                        new()
                        {
                            TicketId = Guid.NewGuid(), Title = "t3", Description = "Ticket 3 Description"
                        }
                    }
                },
                new()
                {
                    ColumnId = Guid.NewGuid(),
                    Title = "Done",
                    Tickets = new List<Ticket>
                    {
                        new()
                        {
                            TicketId = Guid.NewGuid(), Title = "t4", Description = "Ticket 4 Description"
                        }
                    }
                }
            }
        };
    }
}