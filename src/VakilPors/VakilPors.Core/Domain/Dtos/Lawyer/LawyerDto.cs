using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos.Lawyer
{
    public record LawyerDto
    {
        public double Rating { get; set; }
        public string ParvandeNo { get; set; }
        public bool IsAuthorized { get; set; }=false;
        public UserDto User { get; set; }
        public byte[] ProfilePicture { get; set; }
        public byte[] ProfileBackgroundPicture { get; set; }
        public bool IsOnline { get; set; }
        public string Name { get; set; }
        public string Field { get; set; }
        public int NumberOfRates { get; set; } = 0;
        public string city { get; set; }
        [Range(1, 3)]
        public int Grade { get; set; }
        [RegularExpression(@"^\d{5}$")]
        public long LicenseNumber { get; set; }
        public List<string> Specialists { get; set; }
        public int YearsOfExperience { get; set; }
        [RegularExpression(@"^(Male|Female)$", ErrorMessage = "Gender must be either Male or Female.")]
        public string Gender { get; set; }
        public string EducationField { get; set; }
        public string OfficeAddress { get; set; }
        public int NumberOfConsultations { get; set; }
        public int NumberOfAnswers { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfVerifies { get; set; }
        public string AboutMe { get; set; }
        public byte[] CallingCard { get; set; }
        public string ResumeLink { get; set; }
    }
}