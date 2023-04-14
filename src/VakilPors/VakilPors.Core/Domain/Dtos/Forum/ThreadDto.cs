﻿

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos;

public class ThreadDto
{
    public int Id { get; set; }

    
    public string Title { get; set; }

    public string Description { get; set; }

    public int LikeCount { get; set; }

    public int UserId { get; set; }
}

