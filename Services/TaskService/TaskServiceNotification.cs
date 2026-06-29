using AI_genda_API.Abstractions.Enums;
using Microsoft.EntityFrameworkCore.Internal;

namespace AI_genda_API.Services.TaskService;

public class TaskServiceNotification(
    AppContext context,
    IEmailSender emailSender,
    ILogger<TaskServiceNotification> logger
    )
    : ITaskServiceNotification
{
    public AppContext _context { get; } = context;
    public IEmailSender _emailSender { get; } = emailSender;
    public ILogger<TaskServiceNotification> _logger { get; } = logger;

    public async System.Threading.Tasks.Task SendTaskNotification()
    {

        var userstasks = await _context.Users
            .Where(x => !x.IsDisabled)
            .Join(_context.Tasks, user => user.Id, task => task.CreatedById, (user, task) => new { user, task })
            .Where(x => x.task.DueDate == DateTime.UtcNow &&
                        x.task.IsActive && x.task.Status == TaskStatuss.Todo || x.task.Status == TaskStatuss.Ongoing
                        && x.task.Priority == TaskPriority.High || x.task.Priority == TaskPriority.Critical)
            .Select(x => new
            {
                UserEmail = x.user.Email,
                UserName = x.user.FirstName + " " + x.user.SecondName,
                TaskTitle = x.task.Title,
                TaskDueDate = x.task.DueDate
            })
            .GroupBy(x => x.UserEmail)
            .ToListAsync();


        userstasks.ForEach(async row => {

            var userEmail = row.Key;
            var TaskMetaDate = ""; 

            foreach (var item in row)
            {
                TaskMetaDate += item.TaskTitle +". DueDate: " + item.TaskDueDate.ToString() + "<br>"; 
            }

            var Placeholders = new Dictionary<string, string>
            {
                    { "{{username}}", userEmail ?? "AiGenda User"},
                    { "{{title}}", TaskMetaDate?? "No Task" }                    
            };

            if (!string.IsNullOrEmpty(TaskMetaDate))
            {
                var MessageBody = EmailBodyBuilder.GenerateEmailBody("TaskDueTodayHighPriority", Placeholders);

                await _emailSender.SendEmailAsync(userEmail!, "📣 AiGenda:Daily Reminder", MessageBody);
            }

        });


    }
}
