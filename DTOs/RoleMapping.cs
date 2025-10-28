namespace N10.DTOs;

public static class RoleMapping
{
    // GetAllListAsync() - Expression for EF projections
    public static Expression<Func<IdentityRole<Guid>, RoleDto>> ToDtoExpression => x => new()
    {
        Id = x.Id,
        Name = x.Name ?? string.Empty,
        NormalizedName = x.NormalizedName ?? string.Empty,
        ConcurrencyStamp = x.ConcurrencyStamp ?? string.Empty
    };

    // Cache compiled projector to avoid repeated Compile() cost
    private static readonly Func<IdentityRole<Guid>, RoleDto> _projector = ToDtoExpression.Compile();

    // GetAllListAsync() - Convenience: map IEnumerable using cached compiled projector (for in-memory mapping)
    public static List<RoleDto> ToDtoList(this IEnumerable<IdentityRole<Guid>> entities) => entities.Select(_projector).ToList();

    // GetByIdAsync() - Map a single materialized entity to DTO (use when you already have an entity instance)
    public static RoleDto ToDto(this IdentityRole<Guid> entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name ?? string.Empty,
        NormalizedName = entity.NormalizedName ?? string.Empty,
        ConcurrencyStamp = entity.ConcurrencyStamp ?? string.Empty
    };

    // AddAsync() - Convert input DTO to IdentityRole entity
    public static IdentityRole<Guid> ToEntity(this RoleInput input) => new(input.Name);

    // UpdateAsync() - Update existing entity from input
    public static void UpdateFromInput(this IdentityRole<Guid> entity, RoleInput input)
    {
        entity.Name = input.Name;
    }

    // UI -> Convert RoleDto to RoleInput
    public static RoleInput ToInput(this RoleDto dto) => new() { Name = dto.Name };
}