using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using GroupChatActor.Interfaces;
using GroupChatActor.Interfaces.Models;

namespace GroupChatActor
{
	/// <remarks>
	/// This class represents an actor.
	/// Every ActorID maps to an instance of this class.
	/// The StatePersistence attribute determines persistence and replication of actor state:
	///  - Persisted: State is written to disk and replicated.
	///  - Volatile: State is kept in memory only and replicated.
	///  - None: State is kept in memory only and not replicated.
	/// </remarks>
	[StatePersistence(StatePersistence.Persisted)]
	internal class GroupChat : Actor, IGroupChat
	{
		private const string StateName = "messages";

		/// <summary>
		/// Initializes a new instance of GroupChat
		/// </summary>
		/// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
		/// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
		public GroupChat(ActorService actorService, ActorId actorId)
			: base(actorService, actorId)
		{
		}

		/// <summary>
		/// This method is called whenever an actor is activated.
		/// An actor is activated the first time any of its methods are invoked.
		/// </summary>
		protected override Task OnActivateAsync()
		{
			ActorEventSource.Current.ActorMessage(this, "Actor activated.");

			return StateManager.TryAddStateAsync(StateName, new List<Message>(), CancellationToken.None);
		}

		/// <summary>
		/// Adds a message to this actor's state.
		/// </summary>
		/// <param name="particpantId"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public async Task AddMessageAsync(Guid particpantId, string message)
		{
			var data = new Message
			{
				ParticipantId = particpantId,
				Text = message
			};

			//Retrieve the current state
			var currentState = await StateManager.GetStateAsync<List<Message>>(StateName, CancellationToken.None);

			//Need to create a separate instance of the list to modify
			var newList = currentState.ToList();
			newList.Add(data);

			//Save this back to the state
			await StateManager.SetStateAsync(StateName, newList, CancellationToken.None);

			//If we were actually building this, out, we'd have something here to send the messages off to all the other participants too - but.. alas.
		}

		/// <summary>
		/// Retrieves all the messages associated with this actor.
		/// </summary>
		/// <returns></returns>
		public async Task<List<Message>> GetMesagesAsync()
		{
			//Retrieve the current state
			return await StateManager.GetStateAsync<List<Message>>(StateName, CancellationToken.None);
		}
	}
}
