using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Comment
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string Content { get; set; } = null!;

    public bool? IsApproved { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? NewsId { get; set; }

    public int? PageId { get; set; }

    public virtual News? News { get; set; }

    public virtual Page? Page { get; set; }
}
