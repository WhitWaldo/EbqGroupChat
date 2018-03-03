using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChatsService.Models
{
	[DataContract]
    public class Room
    {
		[DataMember(Name="RoomId", Order=100)]
        public Guid RoomId { get; set; }

		[DataMember(Name="Participants", Order =200 )]
		public List<Guid> Participants { get; set; }
    }
}
