using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MessengerServer.Models;

namespace MessengerMiniApp.DTOs
{
    public class MessageDto : INotifyPropertyChanged
    {
        [JsonPropertyName("messageId")]
        public int MessageId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("userID")]
        public int UserID { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("fileId")]
        public int? FileId { get; set; }

        [JsonPropertyName("fileType")]
        public string? FileType { get; set; }

        [JsonPropertyName("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;
        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        // ↓↓↓ Изменили MessageStatus на int ↓↓↓
        private int _status;
        [JsonPropertyName("status")]
        public int Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isCached;
        [JsonIgnore]
        public bool IsCached
        {
            get => _isCached;
            set
            {
                if (_isCached != value)
                {
                    _isCached = value;
                    OnPropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public string Time => CreatedAt.ToLocalTime().ToString("HH:mm");

        [JsonIgnore]
        public bool IsFileMessage => !string.IsNullOrWhiteSpace(FileUrl);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
