using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class StatItem
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Value { get; set; }

    public string? Description { get; set; }

    public bool IsVisible { get; set; } = true;

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
