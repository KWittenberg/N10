namespace N10.Repository.Interfaces;

public interface IRoleRepository
{
    Task<Result<List<RoleDto>>> GetAllAsync();

    Task<Result<RoleDto>> GetByIdAsync(int id);

    Task<Result> AddAsync(RoleInput input);

    Task<Result> UpdateAsync(int id, RoleInput input);

    Task<Result> DeleteAsync(int id);
}