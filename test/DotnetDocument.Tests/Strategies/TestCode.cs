namespace DotnetDocument.Tests.Strategies
{
    /// <summary>
    /// The test code class
    /// </summary>
    public static class TestCode
    {
        /// <summary>
        /// The without doc class
        /// </summary>
        public static class WithoutDoc
        {
            /// <summary>
            /// The simple
            /// </summary>
            public const string SimpleClass = @"
    public class UserRepository
    {
        public User Get(string id)
        {
            return new User();
        }
    }";

            /// <summary>
            /// The simple
            /// </summary>
            public const string SimpleClassLeftAlign = @"
public class UserRepository
{
    public User Get(string id)
    {
        return new User();
    }
}";

            /// <summary>
            /// The class with inheritance
            /// </summary>
            public const string ClassWithInheritance = @"
    public class UserRepository : RepositoryBase<User>, IUserRepository where User : Entity
    {
        public User Get(string id)
        {
            return new User();
        }
    }";

            /// <summary>
            /// The block ctor
            /// </summary>
            public const string BlockCtor = @"
public class UserService
{
        public UserService(ILogger<UserRepository> logger, IUserRepository userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(""Logger can't be null"");            
            _userRepository = userRepository ?? throw new ArgumentNullException(""User repository can't be null"");
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

            var name = ""hello"";
            if(false)
            {
                throw new YetAnotherException($""This name is {name}"");
            }

            if(false)
            {
                throw ExceptionsHelper.Create($""This name is {name}"");
            }

            try
            {
                _userRepository.Initialize();
            } 
            catch(Exception e)
            {
                throw;
            }

            try
            {
                _userRepository.Initialize();
            } 
            catch(Exception e)
            {
                throw new StrangeException(e, ""Something went wrong"");
            }

            throw new NotImplementedException(""This method is not implemented"");
        }
}";
        }

        /// <summary>
        /// The with doc class
        /// </summary>
        public static class WithDoc
        {
            /// <summary>
            /// The simple
            /// </summary>
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
            /// <summary>
            /// The simple
            /// </summary>
            public const string SimpleClassLeftAlign = @"
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

            /// <summary>
            /// The class with inheritance
            /// </summary>
            public const string ClassWithInheritance = @"
    /// <summary>
    /// The user repository class
    /// </summary>
    /// <seealso cref=""RepositoryBase{User}""/>
    /// <seealso cref=""IUserRepository""/>
    public class UserRepository : RepositoryBase<User>, IUserRepository where User : Entity
    {
        public User Get(string id)
        {
            return new User();
        }
    }";

            /// <summary>
            /// The block ctor
            /// </summary>
            public const string BlockCtor = @"
        /// <summary>
        /// Creates a new instance of the <see cref=""UserService""/> class.
        /// </summary>
        /// <param name=""logger"">The logger.</param>
        /// <param name=""userRepository"">The user repository.</param>
        /// <exception cref=""ArgumentNullException""></exception>
        /// <exception cref=""ArgumentNullException"">Logger can't be null</exception>
        /// <exception cref=""StrangeException"">Something went wrong</exception>
        /// <exception cref=""NotImplementedException"">This method is not implemented</exception>
        /// <exception cref=""YetAnotherException"">This name is {name}</exception>
        /// <exception cref=""ArgumentNullException"">User repository can't be null</exception>
        public UserService(ILogger<UserRepository> logger, IUserRepository userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(""Logger can't be null"");            
            _userRepository = userRepository ?? throw new ArgumentNullException(""User repository can't be null"");
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

            var name = ""hello"";
            if(false)
            {
                throw new YetAnotherException($""This name is {name}"");
            }

            if(false)
            {
                throw ExceptionsHelper.Create($""This name is {name}"");
            }

            try
            {
                _userRepository.Initialize();
            } 
            catch(Exception e)
            {
                throw;
            }

            try
            {
                _userRepository.Initialize();
            } 
            catch(Exception e)
            {
                throw new StrangeException(e, ""Something went wrong"");
            }

            throw new NotImplementedException(""This method is not implemented"");
        }";
        }
    }
}
