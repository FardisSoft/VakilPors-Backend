using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities
{
    public class Lawyer:IEntity
    {
        [Key]
        public int Id { get; set; }
        public double Rating { get; set; }=0d;
        public string ProfileImageUrl { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        [StringLength(5)]
        public string LicenseNumber  { get; set; }
        public string MemberOf { get; set; }
        public int YearsOfExperience { get; set; }
        public string OfficeAddress { get; set; }
        public string Education { get; set; }
        public string AboutMe { get; set; }
        public string CallingCardImageUrl { get; set; }
        public string ResumeLink { get; set; }
        public string Specialties { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string ProfileBackgroundPictureUrl {get; set; }
        public int NumberOfRates { get; set; } 
        public string Gender { get; set; }
        public int Tokens { get; set; }
        public bool IsVerified { get; set; } = false;
        public string NationalCardImageUrl { get; set; } 
        public virtual ICollection<DocumentAccess> DocumentAccesses { get; set; }
    }
}