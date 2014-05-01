using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueSpy.Api
{
	public interface IUserService
	{
		User GetLoggedInUser();
		User GetUserByEmail (string email);
		bool UserExists (string email);
		IEnumerable<User> GetAllUsers();
		bool IsValidUser (string email, string password);
		bool IsLoggedIn();
	}

	public class UserService : IUserService
	{
		private User loggedInUser;
		private readonly IPasswordService passwordService;
		private readonly IDbReader dbReader;

		public UserService (IPasswordService passwordService, IDbReader dbReader)
		{
			Preconditions.CheckNotNull (passwordService, "passwordService");
			Preconditions.CheckNotNull (dbReader, "dbReader");

			this.passwordService = passwordService;
			this.dbReader = dbReader;
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
			var user = dbReader.Get<User> ("Email = :Email", x => x.Email = email).SingleOrDefault ();
			if (user == null) {
				throw new UserNotFoundException ();
			}
			return user;
		}

		public bool UserExists(string email)
		{
			Preconditions.CheckNotNull (email, "email");
			var user = dbReader.Get<User> ("Email = :Email", x => x.Email = email).SingleOrDefault ();
			return (user != null);
		}

		public IEnumerable<User> GetAllUsers()
		{
			return dbReader.Get<User> ();
		}
	}


	public class UserNotFoundException : Exception
	{

	}
}

