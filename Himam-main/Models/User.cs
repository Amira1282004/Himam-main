using System;
using System.Collections.Generic;

namespace Himam_main.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
