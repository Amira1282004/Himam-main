using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class ServiceCategory
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int? SortOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
