using System;
using System.Collections.Generic;

namespace Test1.Models;

public partial class Book
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? BookName { get; set; }

    public string? Author { get; set; }

    public string? IsAvailable { get; set; }

    public int? Price { get; set; }

    public int? ShelfId { get; set; }

    public string? Status { get; set; }
}
