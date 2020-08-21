using Domain.Commands;
using Domain.Queries;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebClient.Abstractions;

namespace WebClient.Services
{
    public class TaskManagerService : ITaskManagerService
    {
        private HttpClient _httpClient;
        public TaskManagerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CreateTaskCommandResult> Create(CreateTaskCommand command)
        {
            return await _httpClient.PostJsonAsync<CreateTaskCommandResult>("tasks", command);
        }

        public async Task<GetAllTasksQueryResult> GetAllTasks()
        {
            return await _httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }

        public async Task<UpdateTaskCommandResult> AssignTask(UpdateTaskCommand task)
        {
            return await _httpClient.PutJsonAsync<UpdateTaskCommandResult>($"tasks/AssignTask", task);
        }

       
    }
}
