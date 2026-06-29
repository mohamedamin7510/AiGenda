namespace AI_genda_API.Errors;

    public static class TaskErrors
    {
        public static readonly Error TaskNotFound =
              new("Task.NotFound", "Task not found or has been deleted.", StatusCodes.Status404NotFound);

        public static readonly Error AlreadyAssigned = 
             new("Task.AlreadyAssigned", "This member is already assigned to the task.", StatusCodes.Status409Conflict);

        public static readonly Error AssigneeNotFound =
             new("Task.AssigneeNotFound", "Assignee not founded on this task.", StatusCodes.Status404NotFound);

    }

