using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Medium
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string? FileType { get; set; }

    public int? FileSize { get; set; }

    public DateTime? UploadedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
