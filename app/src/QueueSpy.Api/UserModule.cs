using System;
using System.Collections.Generic;
using Nancy;
using Npgsql;
using System.Data;

namespace QueueSpy.Api
{
	public class UserModule : NancyModule
	{
		private string connectionString = "Server=127.0.0.1;Port=5432;User Id=mike;Password=;Database=mike";

		public UserModule ()
		{
			Get ["/user/"] = parameters => GetAllUsers ();
		}

		public IEnumerable<User> GetAllUsers()
		{
			var users = new List<User> ();

			using (var connection = new NpgsqlConnection (connectionString)) 
			{
				connection.Open ();
				var command = new NpgsqlCommand ("select Id, Username, PasswordHash from \"User\"", connection);
				var reader = command.ExecuteReader ();
				while (reader.Read ()) 
				{
					var user = new User (reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
					users.Add (user);
				}
			}

			return users;
		}
	}

	public class User
	{
		public int Id { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }

		public User(int id, string username, string password)
		{
			Id = id;
			Username = username;
			Password = password;
		}
	}
}

