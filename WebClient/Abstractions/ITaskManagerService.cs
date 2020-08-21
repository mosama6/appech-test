using Domain.Commands;
using Domain.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Abstractions
{
    public interface ITaskManagerService
    {
        Task<CreateTaskCommandResult> Create(CreateTaskCommand command);
        Task<GetAllTasksQueryResult> GetAllTasks();
        Task<UpdateTaskCommandResult> AssignTask(UpdateTaskCommand task);
        
    }
}
