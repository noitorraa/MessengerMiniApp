using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public partial class MediaFile
{
    [JsonPropertyName("fileId")]
    public int FileId { get; set; }
    [JsonPropertyName("senderId")]
    public int? SenderId { get; set; }
    [JsonPropertyName("chatId")]
    public int? ChatId { get; set; }
    [JsonPropertyName("fileUrl")]
    public string FileUrl { get; set; } = null!;
    [JsonPropertyName("fileType")]
    public string FileType { get; set; } = null!;
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User? Sender { get; set; }
}
