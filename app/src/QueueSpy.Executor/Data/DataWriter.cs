using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Npgsql;

namespace QueueSpy.Executor
{
	public interface IDataWriter
	{
		int Insert<T> (T model) where T : class, IModel;
		void Delete<T> (int id) where T : class, IModel;
		void Update<T> (int id, Action<dynamic> updator) where T : class, IModel;
	}

	public class DataWriter : IDataWriter
	{
		private readonly string connectionString = "";

		public DataWriter ()
		{
			connectionString = ConfigurationManager.ConnectionStrings ["queueSpyDb"].ConnectionString;
		}

		public int Insert<T>(T model) where T : class, IModel
		{
			var properties = typeof(T).GetProperties ().Where (x => x.Name != "Id");
			var columnList = string.Join (", ", properties.Select (x => string.Format ("{0}", x.Name)));
			var valuesList = string.Join (", ", properties.Select (x => string.Format (":{0}", x.Name)));
			var sql = string.Format ("INSERT INTO \"{0}\" ({1}) VALUES({2}) RETURNING id;", typeof(T).Name, columnList, valuesList);
			var parameters = properties.ToDictionary<PropertyInfo, string, object> (x => x.Name, x => x.GetValue (model));

			return ExecuteSqlNonQuery (sql, parameters);
		}

		public void Delete<T> (int id) where T : class, IModel
		{
			var sql = string.Format ("DELETE FROM \"{0}\" WHERE id = {1};", typeof(T).Name, id);
			ExecuteSqlNonQuery (sql);
		}

		public int ExecuteSqlNonQuery(string sql, IDictionary<string, object> parameters = null)
		{
			using (var connection = new NpgsqlConnection (connectionString)) {
				connection.Open ();
				var command = new NpgsqlCommand (sql,connection);
				AddParameters (command, parameters);
				return (int)(command.ExecuteScalar () ?? 0);
			}
		}

		public void AddParameters(NpgsqlCommand command, IDictionary<string, object> paramters = null)
		{
			if (paramters == null)
				return;
			foreach (var parameter in paramters) {
				command.Parameters.AddWithValue (parameter.Key, parameter.Value);
			}
		}

		public void Update<T> (int id, Action<dynamic> updator) where T : class, IModel
		{
			var properties = GetProperties (updator);
			var sets = string.Join (", ", properties.Select (x => string.Format("{0} = :{0}", x.Key)));
			var sql = string.Format ("UPDATE \"{0}\" SET {1} WHERE Id = {2}", typeof(T).Name, sets, id);
			ExecuteSqlNonQuery (sql, properties);
		}

		IDictionary<string, object> GetProperties (Action<dynamic> updator)
		{
			var properties = new ExpandoObject ();
			updator (properties);
			return properties;
		}
	}

	public interface IModel
	{
		int Id { get; }
	}
}

