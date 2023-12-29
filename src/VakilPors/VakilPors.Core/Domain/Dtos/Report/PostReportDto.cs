using System.ComponentModel.DataAnnotations;

namespace VakilPors.Core.Domain.Dtos.Report
{
    public record PostReportDto
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CommentId{ get; set; }
    }
}
