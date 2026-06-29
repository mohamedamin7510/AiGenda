using Task = System.Threading.Tasks.Task;

namespace AI_genda_API.Services.TaskService;

public interface ITaskServiceNotification
{
    public Task SendTaskNotification(); 

}
