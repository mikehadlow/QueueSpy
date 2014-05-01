using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Reflection;
using Npgsql;

namespace QueueSpy.Api
{
	public interface IDbReader
	{
		T GetById<T>(int id) where T : class, IModel, new();
		IEnumerable<T> Get<T> (string whereClause = null, Action<dynamic> parameters = null) where T : class, IModel, new();
	}

	public class DbReader : IDbReader
	{
		private readonly string connectionString = "";
		public DbReader ()
		{
			connectionString = ConfigurationManager.ConnectionStrings ["queueSpyDb"].ConnectionString;
		}

		public T GetById<T> (int id) where T : class, IModel, new()
		{
			var model = Get<T> ("Id = :Id", x => x.Id = id).SingleOrDefault ();
			if(model == null) {
				throw new RowNotFoundInTableException (string.Format ("No record for Id = {0} in table {1}", id, typeof(T).Name));
			}
			return model;
		}

		public IEnumerable<T> Get<T> (string whereClause = null, Action<dynamic> parameters = null) where T : class, IModel, new()
		{
			var parameterString = string.Join (", ", typeof(T).GetProperties ().Select (x => x.Name));
			var sql = string.Format ("select {0} from \"{1}\" {2}", parameterString, typeof(T).Name, whereClause == null ? "" : "where " + whereClause);
			Console.WriteLine ("About to run sql: '{0}'", sql);

			using (var connection = new NpgsqlConnection (connectionString)) {
				connection.Open ();
				var command = new NpgsqlCommand (sql,connection);
				AddParameters (command, parameters);
				var reader = command.ExecuteReader ();
				while (reader.Read ()) {
					yield return CreateInstanceFromDataReader<T> (reader);
				}
			}
		}

		public void AddParameters(NpgsqlCommand command, Action<dynamic> paramters)
		{
			if (paramters == null)
				return;
			var parameterList = new ExpandoObject ();
			paramters (parameterList);
			foreach (var parameter in (IDictionary<string, object>)parameterList) {
				command.Parameters.AddWithValue (parameter.Key, parameter.Value);
			}
		}

		public T CreateInstanceFromDataReader<T>(IDataReader reader) where T : class, IModel, new()
		{
			var model = new T ();
			foreach (var property in typeof(T).GetProperties()) {
				property.SetValue (model, reader [property.Name]);
			}
			return model;
		}
	}
			
	public class RowNotFoundInTableException : Exception
	{
		public RowNotFoundInTableException(string message) : base(message) {
		}
	}
}

