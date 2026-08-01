using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Testimonial
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Position { get; set; }

    public string Content { get; set; } = null!;

    public int? Rating { get; set; }

    public string? Image { get; set; }

    public DateTime? CreatedAt { get; set; }
}
