using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatsService.Interfaces;
using ChatsService.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ChatsService
{
	/// <summary>
	/// An instance of this class is created for each service replica by the Service Fabric runtime.
	/// </summary>
	internal sealed class ChatsService : StatefulService, IChatsService
	{
		private const string DictionaryName = "data";
		private const string Key = "rooms";

		public ChatsService(StatefulServiceContext context)
			: base(context)
		{ }

		/// <summary>
		/// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
		/// </summary>
		/// <remarks>
		/// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
		/// </remarks>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			return new[]
			{
				new ServiceReplicaListener(this.CreateServiceRemotingListener, "ServiceEndpoint")
			};
		}

		/// <summary>
		/// This is the main entry point for your service replica.
		/// This method executes when this replica of your service becomes primary and has write status.
		/// </summary>
		/// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			var data = await StateManager.GetOrAddAsync<IReliableDictionary2<string, List<Room>>>(DictionaryName);

			using (var tx = StateManager.CreateTransaction())
			{
				var roomList = new List<Room>
				{
					new Room
					{
						RoomId = Guid.NewGuid(),
						Participants = new List<Guid>()
					},
					new Room
					{
						RoomId = Guid.NewGuid(),
						Participants = new List<Guid>()
					},
					new Room
					{
						RoomId = Guid.NewGuid(),
						Participants = new List<Guid>()
					}
				};

				//This will attempt to add the seed data if it doesn't already exist
				await data.TryAddAsync(tx, Key, roomList);

				await tx.CommitAsync();
			}
		}

		public async Task<List<Guid>> GetRooms()
		{
			using (var tx = StateManager.CreateTransaction())
			{
				var dict = await StateManager.GetOrAddAsync<IReliableDictionary2<string, List<Room>>>(tx, DictionaryName);

				//Retrieve the list of rooms
				var listResult = await dict.TryGetValueAsync(tx, Key);

				return !listResult.HasValue ? new List<Guid>() : listResult.Value.Select(a => a.RoomId).ToList();
			}
		}
		
		public async Task AssignToRoom(Guid roomId, Guid participantId)
		{
			//Retrieve the dictionary
			using (var tx = StateManager.CreateTransaction())
			{
				var dict = await StateManager.GetOrAddAsync<IReliableDictionary2<string, List<Room>>>(tx, DictionaryName);

				//Retrieve the list
				var listResult = await dict.TryGetValueAsync(tx, Key);
				var listVal = listResult.Value;
				if (!listResult.HasValue)
				{
					listVal = new List<Room>();
				}
				
				//Make a new clone of the list
				var list = listVal.ToList();

				foreach (var room in list)
				{
					if (room.RoomId == roomId)
					{
						//Only add the participant if the list doesn't already contain it
						if (!room.Participants.Contains(participantId))
						{
							room.Participants.Add(participantId);
						}

						break;
					}
				}

				//Save this back to the dictionary
				await dict.SetAsync(tx, Key, list);

				await tx.CommitAsync();
			}
		}

		public async Task<List<Guid>> GetParticipantsInRoom(Guid roomId)
		{
			using (var tx = StateManager.CreateTransaction())
			{
				var dict = await StateManager.GetOrAddAsync<IReliableDictionary2<string, List<Room>>>(tx, DictionaryName);

				var listResult = await dict.TryGetValueAsync(tx, Key);
				if (!listResult.HasValue)
				{
					return new List<Guid>();
				}

				return listResult.Value.First(a => a.RoomId == roomId).Participants;
			}
		}
	}
}
