﻿using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Domain.Dtos.Case;

public class DocumentStatusUpdateDto
{
    public int DocumentId { get; set; }
    public Status DocumentStatus { get; set; }
}