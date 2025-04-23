using System.Collections.Generic;

namespace ProjectManagerMvc.Models
{
    public class DashboardViewModel
    {
        public int TotalProposals { get; set; }
        public int PendingApprovals { get; set; }
        public int RecentlyReviewed { get; set; }
        public int RecentlyCommented { get; set; }
        public List<DashboardActivity> RecentActivities { get; set; }
    }

    public class DashboardActivity
    {
        public string Description { get; set; }
        public string TimeAgo { get; set; }
    }
}