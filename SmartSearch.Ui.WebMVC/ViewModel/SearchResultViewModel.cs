namespace SmartSearch.UI.WebMVC.ViewModel;

public class SearchResultViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Language { get; set; }
    public int TopicNumber { get; set; }
    public decimal TopicProbabilty { get; set; }
    public bool? IsInCorpus { get; set; }
    public bool? IsInClipText { get; set; }
    public List<decimal>? ClipStart { get; set; }
    public List<decimal>? ClipDuration { get; set; }
}
