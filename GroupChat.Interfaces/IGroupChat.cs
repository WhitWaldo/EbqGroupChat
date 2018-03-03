using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GroupChatActor.Interfaces.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListener = RemotingListener.V2Listener, RemotingClient = RemotingClient.V2Client)]
namespace GroupChatActor.Interfaces
{
	/// <summary>
	/// This interface defines the methods exposed by an actor.
	/// Clients use this interface to interact with the actor that implements it.
	/// </summary>
	public interface IGroupChat : IActor
	{
		/// <summary>
		/// Adds a message to this actor's state.
		/// </summary>
		/// <param name="particpantId">Participant identifier</param>
		/// <param name="message">Message to send</param>
		/// <returns></returns>
		Task AddMessageAsync(Guid particpantId, string message);

		/// <summary>
		/// Retrieves all the messages associated with this actor.
		/// </summary>
		/// <returns></returns>
		Task<List<Message>> GetMesagesAsync();
	}
}
