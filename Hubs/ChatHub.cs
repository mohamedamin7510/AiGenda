using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace AI_genda_API.Hubs;

//public class ChatHub(UserManager<ExtendedUser> userManager , AppContext context):Hub
//{
//    private readonly UserManager<ExtendedUser> _UserManager = userManager;
//    private readonly AppContext _Context = context;

//    //private static readonly ConcurrentDictionary<string, OnlineUserDto> onlineUsers = new();
//    public async Task SendNotification(NotificationDto dto)
//    {
//        var notification = new Notification
//        {
//            UserId = dto.UserId,
//            Title = dto.Title,
//            Message = dto.Message,
//            IsRead = false,
//            CreatedAt = DateTime.UtcNow
//        };

//        context.Notifications.Add(notification);
//        await context.SaveChangesAsync();

//        // Send Real Time Notification
//        await Clients.User(dto.UserId)
//            .SendAsync("ReceiveNotification", new
//            {
//                notification.Id,
//                notification.Title,
//                notification.Message,
//                notification.CreatedAt
//            });

//        // Send Email
//        var user = await context.Users.FindAsync(dto.UserId);

//        if (user?.Email != null)
//        {
//            await emailSender.SendEmailAsync(
//                user.Email,
//                dto.Title,
//                dto.Message
//            );
//        }
//    }



//}
