
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos;

public class ThreadCommentDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int LikeCount { get; set; }
    public UserDto User { get; set; }
    public int ThreadId { get; set; }
}

