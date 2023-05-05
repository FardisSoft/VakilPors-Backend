

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace VakilPors.Core.Domain.Dtos.Case
{
    public class LegalDocumentDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FileUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DocumentCategory { get; set; }

        public int MinimumBudget { get; set; }

        public int MaximumBudget { get; set; }

        public IFormFile File { get; set; }
    }
}
