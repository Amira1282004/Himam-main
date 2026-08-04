using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class News
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ContentAr { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public bool IsFeatured { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int? SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int? UserId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual User? User { get; set; }
}
