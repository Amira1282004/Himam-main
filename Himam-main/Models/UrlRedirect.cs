using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Himam_main.Models;

[Table("UrlRedirects")]
public class UrlRedirect
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string OldUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string NewUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? ExpiredAt { get; set; }

    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
