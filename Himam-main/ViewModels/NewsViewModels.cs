namespace Himam_main.ViewModels;

public class NewsListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Image { get; set; }
    public string Category { get; set; } = "news";
    public string CategoryLabel { get; set; } = "خبر";
    public DateTime? PublishedAt { get; set; }
    public string FormattedDate { get; set; } = string.Empty;
}

public class NewsDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ContentAr { get; set; }
    public string? Image { get; set; }
    public string CategoryLabel { get; set; } = "خبر";
    public DateTime? PublishedAt { get; set; }
    public string FormattedDate { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class NewsPageViewModel
{
    public IReadOnlyList<NewsListItemViewModel> Featured { get; set; } = [];
    public IReadOnlyList<NewsListItemViewModel> All { get; set; } = [];
}
