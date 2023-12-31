﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Inovatec.OfficeManagementTool.Models.Entity
{
    public partial class Comment
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? OrderId { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool? IsDeleted { get; set; }
        public long? ReportId { get; set; }
        public byte Type { get; set; }
        public byte? OrderState { get; set; }

        public virtual OrderRequest Order { get; set; }
        public virtual Report Report { get; set; }
        public virtual User User { get; set; }
    }
}