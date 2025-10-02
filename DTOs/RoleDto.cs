namespace N10.DTOs;

public class RoleDto
{
    public Guid Id { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string ConcurrencyStamp { get; set; } = string.Empty;
}