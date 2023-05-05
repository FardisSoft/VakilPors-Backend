using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities
{
    public class LegalDocument : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public string FileUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DocumentCategory { get; set; }

        public int MinimumBudget { get; set; }

        public int MaximumBudget { get; set; }

        public virtual ICollection<DocumentAccess> Accesses { get; set; }
    }
}
