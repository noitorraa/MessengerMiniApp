using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MessengerServer.Model;

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

        private string _localPath;
        [JsonIgnore]
        public string LocalPath
        {
            get => _localPath;
            set
            {
                if (_localPath != value)
                {
                    _localPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalPath)));
                    // Чтобы XAML пересчитался: одновременно можно принудительно прокинуть IsImageFile
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsImageFile)));
                }
            }
        }

        [JsonIgnore]
        public bool IsCached => !string.IsNullOrEmpty(LocalPath) && System.IO.File.Exists(LocalPath);

        [JsonIgnore]
        public bool IsImageFile => !string.IsNullOrEmpty(FileType) && FileType.StartsWith("image/");
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                }
            }
        }
        [JsonIgnore]
        public string Time => CreatedAt.ToLocalTime().ToString("HH:mm");

        [JsonIgnore]
        public bool IsFileMessage => FileId.HasValue && !string.IsNullOrEmpty(FileName);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
