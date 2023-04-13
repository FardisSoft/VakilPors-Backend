
namespace VakilPors.Core.Domain.Dtos;

public class ThreadWithCommentsDto
{
    public ThreadDto Thread { get; set; }
    public List<ThreadCommentDto> Comments { get; set; }
}

