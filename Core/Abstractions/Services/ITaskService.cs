﻿using Domain.Commands;
using Domain.Queries;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Abstractions.Services
{
    public interface ITaskService
    {
        Task<CreateTaskCommandResult> CreateTaskCommandHandler(CreateTaskCommand command);
        Task<UpdateTaskCommandResult> CompleteTaskCommandHandler(UpdateTaskCommand command);
        Task<UpdateTaskCommandResult> AssignTaskCommandHandler(UpdateTaskCommand command);
        Task<GetAllTasksQueryResult> GetAllTaskQueryHandler();
    }
}
