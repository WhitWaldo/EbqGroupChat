using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatsService.Interfaces;
using GroupChatActor.Interfaces;
using GroupChatActor.Interfaces.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    [Route("api/chat")]
    public class ChatController : Controller
    {
		[HttpPost]
	    public async Task<IActionResult> Post(Guid participantId, Guid roomId, [FromBody]string message)
		{
			var actor = GetActorReference(roomId);

			await actor.AddMessageAsync(participantId, message);

			return Ok();
		}

		[HttpGet]
	    public async Task<IActionResult> Get(Guid roomId)
		{
			var actor = GetActorReference(roomId);

			var messages = await actor.GetMesagesAsync();

			return Ok(JsonConvert.SerializeObject(messages));
		}
		
	    private IGroupChat GetActorReference(Guid roomIdentifier)
	    {
		    return ActorProxy.Create<IGroupChat>(
			    new ActorId(roomIdentifier),
			    new Uri("fabric:/EbqGroupChat/GroupChatActorService"));
	    }

	    private IChatsService GetServiceReference()
	    {
		    return ServiceProxy.Create<IChatsService>(
			    new Uri("fabric:/EbqGroupChat/ChatsService"),
			    ServicePartitionKey.Singleton);
	    }
    }
}
