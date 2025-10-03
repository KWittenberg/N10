namespace N10.Repository.Interfaces;

public interface IUserRepository
{
    Guid GetCurrentUserId();
    //Task<UserDto> GetCurrentUser();
    Task<Result<UserDto>> GetCurrentUserAsync();



    //Task<Result<List<UserDto>>> GetAllToListAsync();
    Task<Result<IQueryable<UserDto>>> GetAllAsync();

    Task<Result<UserDto>> GetByIdAsync(Guid id);




    Task<Result> AddAsync(UserInput input);
    Task<Result> UpdateAsync(Guid id, UserInput input);
    Task<Result> DeleteAsync(Guid id);



    Task<Result> UpdateAvatarAsync(Guid id, AvatarInput input, bool isOriginal);
    Task<Result> DeleteAvatarAsync(Guid id);



    Task<Result<List<RoleDto>>> GetRolesByUserIdAsync(Guid userId);
    Task<Result<List<RoleDto>>> GetRolesNotAssignedToUserAsync(Guid userId);
    Task<Result<RoleDto>> GetRoleByIdAsync(Guid userId, Guid roleId);
    Task<Result> AddRoleAsync(Guid userId, Guid roleId);
    Task<Result> DeleteRoleAsync(Guid userId, Guid roleId);




    Task<bool> IsUserAvailableAsync(string email);
}