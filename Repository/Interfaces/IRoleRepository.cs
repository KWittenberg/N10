namespace N10.Repository.Interfaces;

public interface IRoleRepository
{
    Task<Result<IQueryable<RoleDto>>> GetAllQueryableAsync();

    Task<Result<List<RoleDto>>> GetAllListAsync();

    Task<Result<RoleDto>> GetByIdAsync(Guid id);

    Task<Result> AddAsync(RoleInput input);

    Task<Result> UpdateAsync(Guid id, RoleInput input);

    Task<Result> DeleteAsync(Guid id);
}