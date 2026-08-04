using System;

namespace Himam_main.Models;

public class Event
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public bool IsFeatured { get; set; }
    public int? SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int? UserId { get; set; }
    
    public virtual User? User { get; set; }
}