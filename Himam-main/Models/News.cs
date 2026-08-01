using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class News
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public string? ContentEn { get; set; }

    public string? ContentAr { get; set; }

    public string? Image { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? User { get; set; }
}
