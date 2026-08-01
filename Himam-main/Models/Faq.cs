using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Faq
{
    public int Id { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }
}
