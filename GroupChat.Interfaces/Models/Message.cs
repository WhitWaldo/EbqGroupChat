using System;
using System.Runtime.Serialization;

namespace GroupChatActor.Interfaces.Models
{
	[DataContract]
    public class Message
    {
		/// <summary>
		/// The identifier of the participant.
		/// </summary>
		[DataMember(Name="ParticipantId", Order = 100)]
        public Guid ParticipantId { get; set; }

		/// <summary>
		/// The text of the message.
		/// </summary>
		[DataMember(Name="Text", Order = 200)]
		public string Text { get; set; }
    }
}
