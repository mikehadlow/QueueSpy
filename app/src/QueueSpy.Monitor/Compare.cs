using System;
using System.Linq;
using System.Collections.Generic;
using EasyNetQ;

namespace QueueSpy.Monitor
{
	/// <summary>
	/// A device for comparing two matching data structures.
	/// The model is the current state of an entitry (broker, vhost, queue, connection etc) in the database,
	/// the status is the sampled current state of the broker.
	/// 
	/// This generic comparer visits each entity in turn (the entity relationships are defined by the ChildItertor) and discovers 
	/// where new entities have appeared, and existing entities have disappeared. It also gives an oppurtunity to process
	/// unchanged entities where the state needs to be continually monitored (such as queue levels).
	/// </summary>
	public class Compare<TModel, TStatus, TKey>
	{
		public Func<TModel, TKey> ModelKeySelector { get; set; }
		public Func<TStatus, TKey> StatusKeySelector { get; set; }

		public Action<TModel, CompareContext> OnModelDeleted { get; set; }
		public Action<TStatus, CompareContext> OnNewStatus { get; set; }
		public Action<TModel, TStatus, CompareContext> OnUnchanged { get; set; }

		private readonly IList<Action<TModel, TStatus, CompareContext>> childIterators = new List<Action<TModel, TStatus, CompareContext>>();

		public void AddChildIterator<TChildModel, TChildStatus, TChildKey>(ChildIterator<TModel, TStatus, TChildModel, TChildStatus, TChildKey> childIterator)
		{
			childIterators.Add (childIterator.VisitChildren);
		}

		public IEnumerable<TModel> GetDeletedModels(IEnumerable<TModel> models, IEnumerable<TStatus> statuses)
		{
			return models.Where (m => !statuses.Any (s => ModelKeySelector (m).Equals (StatusKeySelector (s))));
		}

		public IEnumerable<TStatus> GetNewStatuses(IEnumerable<TModel> models, IEnumerable<TStatus> statuses)
		{
			return statuses.Where (s => !models.Any (m => ModelKeySelector (m).Equals (StatusKeySelector (s))));
		}

		public IEnumerable<Pair<TModel, TStatus>> GetUnchanged(IEnumerable<TModel> models, IEnumerable<TStatus> statuses)
		{
			return models.Join (statuses, ModelKeySelector, StatusKeySelector, (m, s) => new Pair<TModel, TStatus> { Model = m, Status = s });
		}

		public void VisitDeletedModel(TModel model, CompareContext context)
		{
			if (OnModelDeleted != null) {
				OnModelDeleted (model, context);
			}
		}

		public void VisitNewStatus(TStatus status, CompareContext context)
		{
			if (OnNewStatus != null) {
				OnNewStatus (status, context);
			}
		}

		public void VisitUnchanged(TModel model, TStatus status, CompareContext context)
		{
			if(OnUnchanged != null) {
				OnUnchanged (model, status, context);
			}

			context.PushParents (model, status);
			foreach(var visitChildren in childIterators) {
				visitChildren (model, status, context);
			}
			context.PopParents ();
		}
	}

	public class ChildIterator<TModel, TStatus, TChildModel, TChildStatus, TChildKey>
	{
		public Func<TModel, IEnumerable<TChildModel>> ChildModelSelector { get; set; }
		public Func<TStatus, IEnumerable<TChildStatus>> ChildStatusSelector { get; set; }
		public Compare<TChildModel, TChildStatus, TChildKey> ChildCompare { get; set; }

		public void VisitChildren(TModel model, TStatus status, CompareContext context)
		{
			var childModels = ChildModelSelector(model);
			var childStatuses = ChildStatusSelector(status);

			var deletedModels = ChildCompare.GetDeletedModels(childModels, childStatuses);
			var newStatuses = ChildCompare.GetNewStatuses(childModels, childStatuses);
			var unchanged = ChildCompare.GetUnchanged (childModels, childStatuses);

			foreach(var deletedModel in deletedModels) {
				ChildCompare.VisitDeletedModel(deletedModel, context);
			}

			foreach(var newStatus in newStatuses) {
				ChildCompare.VisitNewStatus (newStatus, context);
			}

			foreach (var pair in unchanged) {
				ChildCompare.VisitUnchanged (pair.Model, pair.Status, context);
			}
		}
	}

	public class Pair<TModel, TStatus>
	{
		public TModel Model { get; set; }
		public TStatus Status { get; set; }
	}

	public class CompareContext
	{
		public DateTime SampledAt {	get; set; }
		public IBus Bus { get; set; }
		public Action<Messages.BrokerEvent> SendMessage { get; set; }

		private Stack<object> modelStack = new Stack<object> ();
		private Stack<object> statusStack = new Stack<object> ();

		public void PushParents(object parentModel, object parentStatus)
		{
			modelStack.Push (parentModel);
			statusStack.Push (parentStatus);
		}

		public void PopParents()
		{
			modelStack.Pop ();
			statusStack.Pop ();
		}

		public T GetModelParent<T>() where T : class
		{
			return GetParent<T> (modelStack);
		}

		public T GetStatusParent<T>() where T : class
		{
			return GetParent<T> (statusStack);
		}

		private static T GetParent<T>(Stack<object> stack) where T : class
		{
			if(stack.Count == 0) {
				throw new ApplicationException ("context has no parent");
			}

			var parent = stack.Peek () as T;
			if(parent == null) {
				throw new ApplicationException (string.Format ("Expected parent of type {0}, but was {1}",
					typeof(T).Name, stack.Peek ().GetType ().Name));
			}

			return parent;
		}
	}
}

