using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public partial class Chat
{
    [JsonPropertyName("chatId")]
    public int ChatId { get; set; }
    [JsonPropertyName("chatName")]
    public string? ChatName { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
