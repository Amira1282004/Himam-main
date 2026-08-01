using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Message { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public string? RepliedBy { get; set; }

    public DateTime? RepliedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
