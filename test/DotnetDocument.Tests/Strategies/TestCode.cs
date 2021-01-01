namespace DotnetDocument.Tests.Strategies
{
    public static class TestCode
    {
        public static class WithoutDoc
        {
            public const string SimpleClass = @"
    public class UserRepository
    {
        public User Get(string id)
        {
            return new User();
        }
    }";

            public const string ClassWithInheritance = @"
    public class UserRepository : RepositoryBase<User>, IUserRepository where User : Entity
    {
        public User Get(string id)
        {
            return new User();
        }
    }";
        }

        public static class WithDoc
        {
            public const string SimpleClass = @"
    /// <summary>
    /// The user repository class
    /// </summary>
    public class UserRepository
    {
        public User Get(string id)
        {
            return new User();
        }
    }";

            public const string ClassWithInheritance = @"
    /// <summary>
    /// The user repository class
    /// Inherits from RepositoryBase{User} and IUserRepository
    /// </summary>
    public class UserRepository : RepositoryBase<User>, IUserRepository where User : Entity
    {
        public User Get(string id)
        {
            return new User();
        }
    }";
        }
    }
}
