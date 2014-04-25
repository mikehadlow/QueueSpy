namespace QueueSpy.Api
{

	public class User : IModel
	{
		public int Id { get; private set; }
		public string Email { get; private set; }
		public string PasswordHash { get; private set; }
		public string Salt { get; private set; }

		public User(int id, string email, string passwordHash, string salt)
		{
			Preconditions.CheckNotNull (email, "email");
			Preconditions.CheckNotNull (passwordHash, "passwordHash");
			Preconditions.CheckNotNull (salt, "salt");

			Id = id;
			Email = email;
			PasswordHash = passwordHash;
			Salt = salt;
		}
	}
	
}
