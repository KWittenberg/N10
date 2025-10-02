namespace N10.DTOs;

public static class RoleMapping
{
    // GetAllQueryableAsync()
    public static Expression<Func<IdentityRole<Guid>, RoleDto>> ToDtoExpression => x => new RoleDto
    {
        Id = x.Id,
        Name = x.Name ?? string.Empty,
        NormalizedName = x.NormalizedName ?? string.Empty,
        ConcurrencyStamp = x.ConcurrencyStamp ?? string.Empty
    };

    // GetAllListAsync() - Convert IEnumerable<IdentityRole<Guid>> to List<RoleDto>
    public static List<RoleDto> ToDtoList(this IEnumerable<IdentityRole<Guid>> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    // GetByIdAsync() - Convert IdentityRole<Guid> to RoleDto
    public static RoleDto ToDto(this IdentityRole<Guid> entity)
    {
        return new RoleDto
        {
            Id = entity.Id,
            Name = entity.Name ?? string.Empty,
            NormalizedName = entity.NormalizedName ?? string.Empty,
            ConcurrencyStamp = entity.ConcurrencyStamp ?? string.Empty
        };
    }

    // AddAsync() - Convert RoleInput to IdentityRole<Guid>
    public static IdentityRole<Guid> ToEntity(this RoleInput input)
    {
        return new IdentityRole<Guid>(input.Name);
    }

    // UpdateAsync() - Update IdentityRole<Guid> from RoleInput
    public static void UpdateFromInput(this IdentityRole<Guid> entity, RoleInput input)
    {
        entity.Name = input.Name;
    }

    // UI -> Convert RoleDto to RoleInput
    public static RoleInput ToInput(this RoleDto dto)
    {
        return new RoleInput
        {
            Name = dto.Name
        };
    }
}