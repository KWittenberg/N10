namespace N10.Repository.Interfaces;

public interface IRoleRepository
{
    Task<Result<List<RoleDto>>> GetAllAsync();

    Task<Result<RoleDto>> GetByIdAsync(Guid id);

    Task<Result> AddAsync(RoleInput input);

    Task<Result> UpdateAsync(Guid id, RoleInput input);

    Task<Result> DeleteAsync(Guid id);
}