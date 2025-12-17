namespace N10.Services;

public class SeedService(ApplicationDbContext db,
                            IOptions<AdminOptions> adminOptions,
                            IUserStore<ApplicationUser> userStore,
                            UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole<int>> roleManager) : ISeedService
{


    public async Task StartSeedAsync()
    {
#if DEBUG
        await MigrateDatabaseAsync();
#endif

        await SeedRoles();
        await SeedAdminUser();
        //await SeedDemoUsersAsync("../Services/SeedData/DemoUsers.json", 10);
        await SeedDemoUsersAsync("Services/SeedData/DemoUsers.json", 20);
    }

    async Task MigrateDatabaseAsync()
    {
        if ((await db.Database.GetPendingMigrationsAsync()).Any()) await db.Database.MigrateAsync();
    }

    async Task SeedRoles()
    {
        if (await roleManager.FindByNameAsync(ApplicationRole.Admin) is null)
        {
            var adminRole = new IdentityRole<int>(ApplicationRole.Admin);
            var result = await roleManager.CreateAsync(adminRole);
            if (!result.Succeeded) throw new Exception($"Error in creating Role {Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => e.Description))}");

            var role = await roleManager.FindByNameAsync(ApplicationRole.Admin);
            var timeNow = DateTime.Now.ToString("HH:mm:ss");
            if (role is null) throw new Exception($"{timeNow} | Seed ROLE: Fail!");
            Console.WriteLine($"{timeNow} | Seed ROLE: OK - Added - {role?.Name} role!");
        }

        if (await roleManager.FindByNameAsync(ApplicationRole.Customer) is null)
        {
            var userRole = new IdentityRole<int>(ApplicationRole.Customer);
            var result = await roleManager.CreateAsync(userRole);
            if (!result.Succeeded) throw new Exception($"Error in creating Role {Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => e.Description))}");

            var role = await roleManager.FindByNameAsync(ApplicationRole.Customer);
            var timeNow = DateTime.Now.ToString("HH:mm:ss");
            if (role is null) throw new Exception($"{timeNow} | Seed ROLE: Fail!");
            Console.WriteLine($"{timeNow} | Seed ROLE: OK - Added - {role?.Name} role!");
        }

        if (await roleManager.FindByNameAsync(ApplicationRole.User) is null)
        {
            var userRole = new IdentityRole<int>(ApplicationRole.User);
            var result = await roleManager.CreateAsync(userRole);
            if (!result.Succeeded) throw new Exception($"Error in creating Role {Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => e.Description))}");

            var role = await roleManager.FindByNameAsync(ApplicationRole.User);
            var timeNow = DateTime.Now.ToString("HH:mm:ss");
            if (role is null) throw new Exception($"{timeNow} | Seed ROLE: Fail!");
            Console.WriteLine($"{timeNow} | Seed ROLE: OK - Added - {role?.Name} role!");
        }
    }

    async Task SeedAdminUser()
    {
        var adminUser = await userManager.FindByEmailAsync(adminOptions.Value.Email);
        if (adminUser is null)
        {
            var adminRole = await roleManager.FindByNameAsync(ApplicationRole.Admin) ?? throw new Exception("Admin Role Not Found!");
            adminUser = new ApplicationUser
            {
                AvatarUrl = adminOptions.Value.Avatar,

                FirstName = adminOptions.Value.FirstName,
                LastName = adminOptions.Value.LastName,
                CompanyName = adminOptions.Value.CompanyName,
                DateOfBirth = new DateOnly(1973, 10, 11),

                Email = adminOptions.Value.Email,
                EmailConfirmed = true,

                PhoneNumber = adminOptions.Value.PhoneNumber,
                PhoneNumberConfirmed = true,

                Country = adminOptions.Value.Country,
                Zip = adminOptions.Value.Zip,
                City = adminOptions.Value.City,
                Street = adminOptions.Value.Street,

                Latitude = adminOptions.Value.Latitude,
                Longitude = adminOptions.Value.Longitude,
                PlaceId = adminOptions.Value.PlaceId,

                IsActive = true,
                IsDeleted = false,
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow
                //CreatedId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                //LastModifiedId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };

            await userStore.SetUserNameAsync(adminUser, adminOptions.Value.Email, CancellationToken.None);

            var result = await userManager.CreateAsync(adminUser, adminOptions.Value.Password);
            if (!result.Succeeded) throw new Exception($"Error in creating User {Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(adminUser, ApplicationRole.Admin);

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            var timeNow = DateTime.Now.ToString("HH:mm:ss");
            if (user is null) throw new Exception($"{timeNow} | Seed USER: Fail!");

            var isInRoleAdmin = await userManager.IsInRoleAsync(user, ApplicationRole.Admin);
            Console.WriteLine($"{timeNow} | Seed USER: OK - Added - {user?.UserName} user with role Admin!");
        }
    }

    async Task SeedDemoUsersAsync(string filePath, int numberOfUsers)
    {
        var userRole = await roleManager.FindByNameAsync(ApplicationRole.User);
        if (userRole is null) return; //throw new EntityNotFoundException(typeof(IdentityRole), ApplicationRole.User);

        var normalUserExists = await userManager.GetUsersInRoleAsync(userRole.Name);
        var random = new Random();

        if (normalUserExists.Count == 0)
        {
            var jsonText = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<SeedDemoUsers>(jsonText);

            for (int i = 0; i < numberOfUsers; i++)
            {
                var day = random.Next(1, 31);
                var month = random.Next(1, 13);
                var year = random.Next(1990, 2011);
                var dob = new DateOnly(year, month, day);

                var user = new ApplicationUser();

                user.FirstName = data.FirstNames[random.Next(data.FirstNames.Count)];
                user.LastName = data.LastNames[random.Next(data.LastNames.Count)];
                user.UserName = $"{user.FirstName.ToLower()}{user.LastName.ToLower()}";
                user.DateOfBirth = dob;

                user.Email = $"{user.FirstName.ToLower()}.{user.LastName.ToLower()}@gmail.com";
                user.EmailConfirmed = true;

                user.PhoneNumber = $"+38598{random.Next(100000, 999999)}";
                user.PhoneNumberConfirmed = true;

                user.AvatarUrl = $"/img/avatar/{random.Next(1, 11).ToString("D2")}.jpg";

                var location = data.Locations[random.Next(data.Locations.Count)];

                user.Country = location.Country;
                user.Zip = location.Zip;
                user.City = location.City;
                user.Street = location.Street;

                //user.Country = data.Countries[random.Next(data.Countries.Count)];
                //user.Zip = random.Next(10000, 50000).ToString();
                //user.City = data.Cities[random.Next(data.Cities.Count)];
                //user.Street = data.Streets[random.Next(data.Streets.Count)] + " " + random.Next(1, 200);

                user.Latitude = location.Latitude;
                user.Longitude = location.Longitude;
                user.PlaceId = location.PlaceId;

                user.IsActive = true;
                user.IsDeleted = false;
                //user.CreatedId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                user.CreatedUtc = DateTime.UtcNow;
                //user.LastModifiedId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                user.LastModifiedUtc = DateTime.UtcNow;

                var result = await userManager.CreateAsync(user, adminOptions.Value.Password);
                if (!result.Succeeded) throw new Exception($"Error in creating User {Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => e.Description))}");

                await userManager.AddToRoleAsync(user, ApplicationRole.User);
            }
        }

        var usersInUserRole = await userManager.GetUsersInRoleAsync(userRole.Name);
        var userDataCount = usersInUserRole.Count;

        if (userDataCount > 0)
        {
            var timeNow = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"{timeNow} | SEED: Users OK - Added {userDataCount} records");
        }
    }
}