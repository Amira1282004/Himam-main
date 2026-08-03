namespace Himam_main.Models.ViewModels;

public class HomeViewModel
{
    public HeroSection? HeroSection { get; set; }
    public AboutSection? AboutSection { get; set; }
    public List<Sector> Sectors { get; set; } = new();
    public List<CompanyValue> CompanyValues { get; set; } = new();
    public List<ProcessStep> ProcessSteps { get; set; } = new();
    public List<ContactInfo> ContactInfo { get; set; } = new();
    public List<SocialMediaLink> SocialMediaLinks { get; set; } = new();
    public List<StatItem> StatItems { get; set; } = new();
}