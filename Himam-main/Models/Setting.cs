using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string KeyName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "text";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
