using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string? KeyName { get; set; }

    public string? Value { get; set; }

    public string? GroupName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
