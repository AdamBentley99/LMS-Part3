using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string SId { get; set; } = null!;
        public int AId { get; set; }
        public string? Contents { get; set; }
        public uint? Score { get; set; }
        public DateTime Time { get; set; }

        public virtual Assignment AIdNavigation { get; set; } = null!;
        public virtual Student SIdNavigation { get; set; } = null!;
    }
}
