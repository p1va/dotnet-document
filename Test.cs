namespace Test
{
    /// <summary>
    /// The entity repository class
    /// </summary>
    public class EntityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepository"/>
        /// </summary>
        public EntityRepository()
        {
            
        }

        /// <summary>
        /// Gets the user details using the specified id
        /// </summary>
        /// <param name="id">The id</param>
        /// <exception cref="UserNotFoundException">No user found with id {id}</exception>
        /// <exception cref="System.ArgumentNullException">id</exception>
        /// <returns>The user</returns>
        public async Task<User> GetUserDetailsAsync(string id)
        {
            // Load the user from the database
            var user = _repository.FindOneAsync(
                id ?? throw new System.ArgumentNullException("id"));

            if(user is null)
            {
                throw new UserNotFoundException($"No user found with id {id}");
            }

            return user;
        }

    }
}
