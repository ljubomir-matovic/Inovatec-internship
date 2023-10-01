﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Inovatec.OfficeManagementTool.Models.Entity
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Equipment = new HashSet<Equipment>();
            Notifications = new HashSet<Notification>();
            OrderRequests = new HashSet<OrderRequest>();
            Reports = new HashSet<Report>();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ResetTokenExpirationTime { get; set; }
        public DateTime? DateModified { get; set; }
        public bool? IsActive { get; set; }
        public string ResetToken { get; set; }
        public long? OfficeId { get; set; }

        public virtual Office Office { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Equipment> Equipment { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<OrderRequest> OrderRequests { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}