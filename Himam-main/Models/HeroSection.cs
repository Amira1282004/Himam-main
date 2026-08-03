using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class HeroSection
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Subtitle { get; set; }

    public string? CtaText { get; set; }

    public string? CtaLink { get; set; }

    public string? YoutubeVideoId { get; set; }

    public bool IsVideoEnabled { get; set; } = true;

    public bool IsVisible { get; set; } = true;

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
