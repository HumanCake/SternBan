using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public record Board
{
    [BsonId]
    public string BoardId { get; init; }

    public string Title { get; set; }
    public List<Column> Columns { get; set; }


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
                    Title = "To Do",
                    Tickets = new List<Ticket>
                    {
                        new()
                        {
                            Title = "t1", Description = "Ticket 1 Description"
                        },
                        new()
                        {
                            Title = "t2", Description = "Ticket 2 Description"
                        }
                    }
                },
                new()
                {
                    Title = "Doing",
                    Tickets = new List<Ticket>
                    {
                        new()
                        {
                            Title = "t3", Description = "Ticket 3 Description"
                        }
                    }
                },
                new()
                {
                    Title = "Done",
                    Tickets = new List<Ticket>
                    {
                        new()
                        {
                            Title = "t4", Description = "Ticket 4 Description"
                        }
                    }
                }
            }
        };
    }
}