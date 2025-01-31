using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public partial class User
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;
    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; } = null!;
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    public virtual ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
