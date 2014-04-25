using System.Collections.Generic;
using System.Configuration;
using Nancy;
using Npgsql;

namespace QueueSpy.Api
{
	public class UserModule : NancyModule
	{
		private string connectionString = "";

		public UserModule ()
		{
			Initialize ();
			Get ["/user/"] = parameters => GetAllUsers ();
		}

		public void Initialize()
		{
			connectionString = ConfigurationManager.ConnectionStrings ["queueSpyDb"].ConnectionString;
		}

		public IEnumerable<User> GetAllUsers()
		{
			var users = new List<User> ();

			using (var connection = new NpgsqlConnection (connectionString)) 
			{
				connection.Open ();
				var command = new NpgsqlCommand ("select Id, Username, PasswordHash, Salt from \"User\"", connection);
				var reader = command.ExecuteReader ();
				while (reader.Read ()) 
				{
					var user = new User (reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
					users.Add (user);
				}
			}

			return users;
		}
	}
}

