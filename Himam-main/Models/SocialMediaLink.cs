using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class SocialMediaLink
{
    public int Id { get; set; }

    public string? Platform { get; set; }

    public string? Url { get; set; }

    public string? IconSvg { get; set; }

    public bool IsVisible { get; set; } = true;

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
