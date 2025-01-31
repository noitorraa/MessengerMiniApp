using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public partial class ChatMember
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("chatId")]
    public int ChatId { get; set; }
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
