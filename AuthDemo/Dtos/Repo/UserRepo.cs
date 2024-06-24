using AuthDemo.Exceptions;

namespace AuthDemo.Dtos.Repo
{
    public class UserRepo
    {
        private IEnumerable<UserDto> GetAllUsers() => [
                new UserDto {UserName = "AdityaD", UserEmail = "AdityaD@example.com", Roles= [RoleType.Admin, RoleType.Delete]},
                new UserDto {UserName = "John", UserEmail = "john@example.com", Roles= [RoleType.Read, RoleType.Update]},
                new UserDto {UserName = "Jane", UserEmail = "jane@example.com", Roles= [RoleType.Read, RoleType.Update]},
                new UserDto {UserName = "Mike", UserEmail = "mike@example.com", Roles= [RoleType.Read]},
                new UserDto {UserName = "Sarah", UserEmail = "sarah@example.com", Roles= [RoleType.Guest]},
            ];

        public bool ValidateUser(string userName) => GetAllUsers().Any(x => x.UserName == userName) ? true : throw new UserNotFoundException(userName);

        public string[] GetUserPermissions(string userName) => GetAllUsers().First(x => x.UserName == userName).Roles.Select(r => r.ToString()).ToArray();

    }

    public class UserDto
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public RoleType[] Roles { get; set; }
    }

    public enum RoleType
    {
        Admin,
        Read,
        Update,
        Create,
        Delete,
        Guest
    }
}
