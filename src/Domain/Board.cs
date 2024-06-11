namespace Domain;

public record Board
{
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
                        new() { name = "t1", description = "Ticket 1 Description" },
                        new() { name = "t2", description = "Ticket 2 Description" }
                    }
                },
                new()
                {
                    Title = "Doing",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "t3", description = "Ticket 3 Description" }
                    }
                },
                new()
                {
                    Title = "Done",
                    Tickets = new List<Ticket>
                    {
                        new() { name = "t4", description = "Ticket 4 Description" }
                    }
                }
            }
        };
    }
}