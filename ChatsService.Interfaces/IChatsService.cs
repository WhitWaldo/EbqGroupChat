using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace ChatsService.Interfaces
{
    public interface IChatsService : IService
    {
	    Task<List<Guid>> GetRooms();

	    Task AssignToRoom(Guid roomId, Guid participantId);

	    Task<List<Guid>> GetParticipantsInRoom(Guid roomId);
    }
}
