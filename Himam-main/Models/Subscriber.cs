using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Subscriber
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
