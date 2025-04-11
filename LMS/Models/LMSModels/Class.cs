using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Enrollments = new HashSet<Enrollment>();
        }

        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public uint Year { get; set; }
        public string Season { get; set; } = null!;
        public string Location { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ProfessorUid { get; set; } = null!;

        public virtual Course Course { get; set; } = null!;
        public virtual Professor ProfessorU { get; set; } = null!;
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
