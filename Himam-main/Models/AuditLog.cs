using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class AuditLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? UserName { get; set; }

    public string? Action { get; set; }

    public string? OperationType { get; set; } // Create, Update, Delete, Login, Logout, etc.

    public string? EntityType { get; set; } // News, Page, User, Event, etc.

    public int? EntityId { get; set; }

    public string? Details { get; set; }

    public string? ChangesBefore { get; set; } // JSON string of data before modification

    public string? ChangesAfter { get; set; } // JSON string of data after modification

    public bool Success { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; } // Browser/device information

    public DateTime CreatedAt { get; set; }

    public DateTime? ArchivedAt { get; set; } // When the log was archived

    public bool IsArchived { get; set; } = false;

    public virtual User? User { get; set; }
}
