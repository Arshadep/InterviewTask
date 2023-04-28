using System;
using System.Collections.Generic;

namespace Test1.Models;

public partial class Shelf
{
    public int ShelfId { get; set; }

    public string? Code { get; set; }

    public int? RackId { get; set; }
}
