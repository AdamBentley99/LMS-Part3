﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrollment
    {
        public string Student { get; set; } = null!;
        public int ClassId { get; set; }
        public string Grade { get; set; } = null!;

        public virtual Class Class { get; set; } = null!;
        public virtual Student StudentNavigation { get; set; } = null!;
    }
}
