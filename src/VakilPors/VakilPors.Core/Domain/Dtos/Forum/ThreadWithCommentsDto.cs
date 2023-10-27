
using Pagination.EntityFrameworkCore.Extensions;

namespace VakilPors.Core.Domain.Dtos;

public class ThreadWithCommentsDto
{
    public ThreadDto Thread { get; set; }
    public Pagination<ThreadCommentDto> Comments { get; set; }
}

