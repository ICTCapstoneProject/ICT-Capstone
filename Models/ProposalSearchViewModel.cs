using FSSA.Models;
public class ProposalSearchViewModel
{
    public List<MyProposalViewModel> Proposals { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public List<string> StatusFilters { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string> ProjectLevelFilters { get; set; }
    public string SearchKeyword { get; set; }
    public string SortBy { get; set; }
}