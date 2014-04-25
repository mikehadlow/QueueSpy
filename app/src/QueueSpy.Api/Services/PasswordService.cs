using System;
using System.Linq;
using System.Security.Cryptography;

namespace QueueSpy.Api
{
	public interface IPasswordService
	{
		HashResult Hash (string password);
		bool PasswordValidates (string password, string passwordHash, string salt);
	}

	public class PasswordService : IPasswordService
	{
		public PasswordService ()
		{
		}

		/// <summary>
		/// Compute a hash value from the given value and salt.
		/// </summary>
		/// <param name="value">The plaintext value to be hashed.</param>
		/// <param name="salt">The base64 encoded salt.</param>
		/// <returns>The base64 encoded hash</returns>
		public string Hash(string value, string salt)
		{
			var saltBytes = Convert.FromBase64String (salt);
			var valueBytes = System.Text.Encoding.UTF8.GetBytes (value);
			var hashBytes = Hash (valueBytes, saltBytes);
			return Convert.ToBase64String (hashBytes);
		}

		public byte[] Hash(byte[] value, byte[] salt)
		{
			var saltedValue = value.Concat (salt).ToArray ();
			return new SHA256Managed ().ComputeHash (saltedValue);
		}

		public HashResult Hash(string password)
		{
			var salt = GenerateSalt ();
			var hash = Hash (password, salt);
			return new HashResult (hash, salt);
		}

		public bool PasswordValidates(string password, string passwordHash, string salt)
		{
			var newPasswordHash = Hash (password, salt);
			return newPasswordHash == passwordHash;
		}

		public string GenerateSalt()
		{
			var salt = new byte[20];
			using (var randomBytes = new RNGCryptoServiceProvider ()) {
				randomBytes.GetBytes (salt);
				return Convert.ToBase64String (salt);
			}
		}
	}

	public class HashResult
	{
		public string PasswordHash { get; private set; }
		public string Salt { get; private set; }

		public HashResult(string passwordHash, string salt)
		{
			PasswordHash = passwordHash;
			Salt = salt;
		}
	}
}

