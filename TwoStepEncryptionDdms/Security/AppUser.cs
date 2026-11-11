namespace TwoStepEncryptionDdms.Security
{
    public sealed class AppUser
    {
        public int Id { get; }
        public string UserName { get; }
        public string FullName { get; }
        public Permission Permissions { get; }

        public AppUser(int id, string userName, string fullName, Permission permissions)
        {
            Id = id;
            UserName = userName;
            FullName = fullName;
            Permissions = permissions;
        }

        public bool Has(Permission permission)
        {
            return (Permissions & permission) == permission;
        }
    }
}
