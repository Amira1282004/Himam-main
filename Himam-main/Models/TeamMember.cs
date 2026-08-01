using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class TeamMember
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Position { get; set; }

    public string? Bio { get; set; }

    public string? Image { get; set; }

    public string? FacebookLink { get; set; }

    public string? LinkedInLink { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }
}
