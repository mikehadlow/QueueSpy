using System;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Reflection;
using Npgsql;

namespace QueueSpy.Api
{
	public interface IDbReader
	{
		T GetById<T>(int id) where T : class, IModel;
	}

	public class DbReader : IDbReader
	{
		private readonly string connectionString = "";
		public DbReader ()
		{
			connectionString = ConfigurationManager.ConnectionStrings ["queueSpyDb"].ConnectionString;
		}

		public T GetById<T> (int id) where T : class, IModel
		{
			var parameterString = string.Join (", ", typeof(T).GetProperties ().Select (x => x.Name));
			var sql = string.Format ("select {0} from \"{1}\" where Id = {2}", parameterString, typeof(T).Name, id);
			Console.WriteLine ("About to run sql: '{0}'", sql);

			using (var connection = new NpgsqlConnection (connectionString)) {
				connection.Open ();
				var command = new NpgsqlCommand (sql,connection);
				var reader = command.ExecuteReader ();
				T model = null;
				while (reader.Read ()) {
					model = CreateInstanceFromDataReader<T> (reader);
				}
				if (model == null) {
					throw new UserNotFoundException ();
				}
				return model;
			}
		}

		public T CreateInstanceFromDataReader<T>(IDataReader reader) where T : class, IModel
		{
			var constructorParameters = typeof(T).GetProperties().Select<PropertyInfo, object>((x, i) => 
				x.PropertyType == typeof(int) ? (object)reader.GetInt32 (i) : 
				x.PropertyType == typeof(string) ? (object)reader.GetString (i) : 
				(object)null).ToArray();
			return typeof(T).GetConstructors () [0].Invoke (constructorParameters) as T;
		}
	}
}

