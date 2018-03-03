using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatsService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace WebApi.Controllers
{
	[Route("api/room")]
    public class RoomController : Controller
    {
		/// <summary>
		/// Retrieves all the rooms available from the chat service.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
	    public async Task<IActionResult> Get()
		{
			try
			{
				var service = GetServiceReference();

				return Ok(await service.GetRooms());
			}
			catch (Exception ex)
			{
				return BadRequest();
			}
		}

		[HttpGet("roomAssignment")]
	    public async Task<IActionResult> AssignParticipantToRoom(Guid participantId, Guid roomId)
	    {
		    var service = GetServiceReference();

			//First, confirm that the room is one of the available rooms
		    var rooms = await service.GetRooms();
		    if (rooms.All(a => a != roomId))
		    {
				//Room isn't available, according to the service
			    return BadRequest();
		    }

			//Assign the user to the room
		    await service.AssignToRoom(roomId, participantId);
			
		    return Ok();
	    }

		[HttpGet("participants")]
	    public async Task<IActionResult> GetParticipants(Guid roomId)
	    {
		    var service = GetServiceReference();

			//First, confirm that the room is one of the available such rooms
		    var rooms = await service.GetRooms();
		    if (rooms.All(a => a != roomId))
		    {
				//Room isn't available, according to the service
			    return BadRequest();
		    }

		    return Ok(await service.GetParticipantsInRoom(roomId));
	    }


	    private IChatsService GetServiceReference()
	    {
		    return ServiceProxy.Create<IChatsService>(new Uri("fabric:/EbqGroupChat/ChatsService"),
			    ServicePartitionKey.Singleton);
	    }
    }
}
