﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Inovatec.OfficeManagementTool.Models.Entity
{
    public partial class ReportSchedule
    {
        public long Id { get; set; }
        public long OfficeId { get; set; }
        public DateTimeOffset ScheduleDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Office Office { get; set; }
    }
}