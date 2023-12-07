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
    public class DocumentAccess : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public LegalDocument Document { get; set; }

        public Status DocumentStatus { get; set; } = Status.PENDING;
        public int LawyerId { get; set; }

        [ForeignKey(nameof(LawyerId))]
        public Lawyer Lawyer { get; set; }
    }
}
