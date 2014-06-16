using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Reflection;
using Npgsql;

namespace QueueSpy
{
	public interface IDbReader
	{
		T GetById<T>(int id) where T : class, IModel, new();

		IEnumerable<T> Get<T> (string whereClause = null, Action<dynamic> parameters = null) where T : class, IModel, new();

		IEnumerable<T> Get<T, J> (string whereClause = null, Action<dynamic> parameters = null) 
			where T : class, IModel, new()
			where J : class, IModel, new();

		IEnumerable<T> Get<T, J1, J2> (string whereClause = null, Action<dynamic> parameters = null) 
			where T : class, IModel, new()
			where J1 : class, IModel, new()
			where J2 : class, IModel, new();
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
			return GetInternal<T>(string.Format("from \"{0}\"", typeof(T).Name), whereClause, parameters);
		}

		public IEnumerable<T> Get<T, J> (string whereClause = null, Action<dynamic> parameters = null) 
			where T : class, IModel, new()
			where J : class, IModel, new()
		{
			var fromClause = string.Format ("from \"{0}\" inner join \"{1}\" on \"{0}\".{1}Id = \"{1}\".id", typeof(T).Name, typeof(J).Name);
			return GetInternal<T> (fromClause, whereClause, parameters);
		}

		public IEnumerable<T> Get<T, J1, J2> (string whereClause = null, Action<dynamic> parameters = null) 
			where T : class, IModel, new() 
			where J1 : class, IModel, new() 
			where J2 : class, IModel, new()
		{
			var fromClause = string.Format ("from \"{0}\" inner join \"{1}\" on \"{0}\".{1}Id = \"{1}\".Id inner join \"{2}\" on \"{1}\".{2}Id = \"{2}\".Id", 
				typeof(T).Name, 
				typeof(J1).Name, 
				typeof(J2).Name);
			return GetInternal<T> (fromClause, whereClause, parameters);
		}

		private IEnumerable<T> GetInternal<T> (string fromClause, string whereClause = null, Action<dynamic> parameters = null) where T : class, IModel, new()
		{
			var typeName = typeof(T).Name;
			var parameterString = string.Join (", ", GetPropertiesOf<T>().Select (x => "\"" + typeName + "\"." + x.Name));

			var sql = string.Format ("select {0} {1} {2} order by id desc limit 1000", parameterString, fromClause, whereClause == null ? "" : "where " + whereClause);
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
			foreach (PropertyInfo property in GetPropertiesOf<T>()) {
				property.SetValue ((object)model, (object)reader [property.Name], null);
			}
			return model;
		}

		public static bool IsCollectionType (Type type)
		{
			return type.IsGenericType &&
			(
				type.GetGenericTypeDefinition () == typeof(List<>) ||
				type.GetGenericTypeDefinition () == typeof(Dictionary<,>)
			);
		}

		public static IEnumerable<PropertyInfo> GetPropertiesOf<T>()
		{
			return typeof(T).GetProperties ().Where (x => !IsCollectionType (x.PropertyType));
		}
	}
			
	public class RowNotFoundInTableException : Exception
	{
		public RowNotFoundInTableException(string message) : base(message) {
		}
	}
}

