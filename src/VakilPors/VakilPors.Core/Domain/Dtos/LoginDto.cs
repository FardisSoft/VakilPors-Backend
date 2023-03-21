namespace VakilPors.Core.Domain.Dtos;

public record LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

