using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos.Lawyer
{
    public record LawyerDto
    {
        public int Id { get; set; }
        public double Rating { get; set; } = 0d;
        public string ParvandeNo { get; set; }
        public bool IsAuthorized { get; set; } = false;
        public string ProfileImageUrl { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public byte Grade { get; set; }
        public string LicenseNumber { get; set; }
        public string MemberOf { get; set; }
        public int YearsOfExperience { get; set; }
        public string OfficeAddress { get; set; }
        public string Education { get; set; }
        public string AboutMe { get; set; }

        public IFormFile CallingCardImage { get; set; }
        public string CallingCardImageUrl { get; set; }
        public string ResumeLink { get; set; }
        public string Specialties { get; set; }
        public UserDto User { get; set; }

        public IFormFile ProfileBackgroundPicture { get; set; }
        public string ProfileBackgroundPictureUrl { get; set; }
        public int NumberOfRates { get; set; }
        public string Gender { get; set; }
        public int NumbereOfConsulations { get; set; }
        public int NumberOfAnswers { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfVerifies { get; set; }
        public List<string> RatesList { get; set; }
    }
}