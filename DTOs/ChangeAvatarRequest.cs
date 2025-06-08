
namespace MessengerMiniApp.DTOs
{
    public class ChangeAvatarRequest
    {
        public int UserId { get; set; }
        public byte[] NewAvatar { get; set; }
    }
    
}