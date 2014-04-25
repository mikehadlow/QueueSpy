using System;
using System.Configuration;
using Npgsql;

namespace QueueSpy.Api
{
	public interface IUserService
	{
		User GetLoggedInUser();
		User GetUserByEmail (string email);
		bool IsValidUser (string email, string password);
		bool IsLoggedIn();
	}

	public class UserService : IUserService
	{
		private User loggedInUser;
		private readonly string connectionString = "";
		private readonly IPasswordService passwordService;

		public UserService (IPasswordService passwordService)
		{
			Preconditions.CheckNotNull (passwordService, "passwordService");
			this.passwordService = passwordService;

			connectionString = ConfigurationManager.ConnectionStrings ["queueSpyDb"].ConnectionString;
		}

		public User GetLoggedInUser()
		{
			return loggedInUser;
		}

		public bool IsLoggedIn()
		{
			return loggedInUser != null;
		}

		public bool IsValidUser(string email, string password)
		{
			Preconditions.CheckNotNull (email, "email");
			Preconditions.CheckNotNull (password, "password");

			try {
				loggedInUser = GetUserByEmail(email);
				return passwordService.PasswordValidates(password, loggedInUser.PasswordHash, loggedInUser.Salt);
			}
			catch (UserNotFoundException) {
				return false;
			}
		}

		public User GetUserByEmail(string email)
		{
			Preconditions.CheckNotNull (email, "email");

			using (var connection = new NpgsqlConnection (connectionString)) {
				connection.Open ();
				var command = new NpgsqlCommand ("select Id, Email, PasswordHash, Salt from \"User\" where Email = ':email'", connection);
				command.Parameters.Add (new NpgsqlParameter ("email", NpgsqlTypes.NpgsqlDbType.Varchar));
				command.Parameters[0].Value = email;
				var reader = command.ExecuteReader ();
				User user = null;
				while (reader.Read ()) {
					user = new User (reader.GetInt32 (0), reader.GetString (1), reader.GetString(2), reader.GetString(3));
				}
				if (user == null) {
					throw new UserNotFoundException ();
				}
				return user;
			}
		}

	}


	public class UserNotFoundException : Exception
	{

	}
}

