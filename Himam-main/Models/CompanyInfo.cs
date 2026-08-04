using System;

namespace Himam_main.Models;

public class CompanyInfo
{
    public int Id { get; set; }
    
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyNameEn { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string TaglineEn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string Favicon { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string CommercialRegister { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int? UserId { get; set; }
    
    public virtual User? User { get; set; }
}