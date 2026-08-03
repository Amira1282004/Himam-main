using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class ContactInfo
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? WorkingHours { get; set; }

    public string? MapEmbedUrl { get; set; }

    public bool IsVisible { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
