using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Partner
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Logo { get; set; }

    public string? WebsiteUrl { get; set; }

    public int? SortOrder { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
