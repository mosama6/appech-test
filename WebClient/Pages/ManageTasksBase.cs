using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Abstractions;

namespace WebClient.Pages
{
    public class ManageTasksBase: ComponentBase
    {
        protected string NewTaskSubject { get; set; }
        protected List<FamilyMember> members = new List<FamilyMember>();
        protected List<TaskModel> allTasks = new List<TaskModel>();
        protected List<MenuItem> leftMenuItem = new List<MenuItem>();
        protected TaskModel[] tasksToShow;
        protected TaskModel taskToAssign;
        protected bool isLoaded;
        protected bool showLister;
        protected bool showCreator;

        [Inject]
        public ITaskManagerService TaskManagerService { get; set; }

        [Inject]
        public IMemberDataService MemberDataService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var membersResponse = await MemberDataService.GetAllMembers();
            if (membersResponse != null && membersResponse.Payload != null && membersResponse.Payload.Any())
            {
                foreach (var item in membersResponse.Payload)
                {
                    members.Add(new FamilyMember()
                    {
                        avtar = item.Avatar,
                        email = item.Email,
                        firstname = item.FirstName,
                        lastname = item.LastName,
                        role = item.Roles,
                        id = item.Id
                    });
                }
            }
            
            var tasksResponse = await TaskManagerService.GetAllTasks();
            if (tasksResponse != null && tasksResponse.Payload != null && tasksResponse.Payload.Any())
            {
                foreach (var task in tasksResponse.Payload)
                {
                    allTasks.Add(new TaskModel()
                    {
                        id = task.Id,
                        isDone = task.IsComplete,
                        member = members.FirstOrDefault(member=> member.id == task.AssignedMemberId),
                        text = task.Subject

                    });
                }
            }

            leftMenuItem.Add( new MenuItem
            {
                label = "All Tasks",
                referenceId = Guid.Empty,
                isActive = true,
                canHandleDrag = false,

                
            });
            leftMenuItem[0].ClickCallback += showAllTasks;
            for (int i = 1; i < members.Count + 1; i++)
            {
                leftMenuItem.Add(new MenuItem
                {
                    iconColor = members[i - 1].avtar,
                    label = members[i - 1].firstname,
                    referenceId = members[i - 1].id,
                    isActive = false,
                    canHandleDrag = true
                });
                leftMenuItem[i].ClickCallback += onItemClick;
            }
            showAllTasks(null, leftMenuItem[0]);
            isLoaded = true;
        }
        protected void onAddItem()
        {
            showLister = false;
            showCreator = true;
            makeMenuItemActive(null);
            StateHasChanged();
        }

        protected void onItemClick(object sender, object e)
        {
            Guid val = (Guid)e.GetType().GetProperty("referenceId").GetValue(e);
            makeMenuItemActive(e);
            if (allTasks != null && allTasks.Count > 0)
            {
                tasksToShow = allTasks.Where(item => {
                    if (item.member != null)
                    {
                        return item.member.id == val;
                    }
                    else
                    {
                        return false;
                    }
                }).ToArray();
            }
            showLister = true;
            showCreator = false;
            StateHasChanged();
        }
        protected void showAllTasks(object sender, object e)
        {
            tasksToShow =  allTasks.ToArray();
            showLister = true;
            showCreator = false;
            makeMenuItemActive(e);
            StateHasChanged();
        }

        protected void makeMenuItemActive(object e)
        {
            foreach (var item in leftMenuItem)
            {
                item.isActive = false;
            }
            if (e != null)
            {
                e.GetType().GetProperty("isActive").SetValue(e, true);
            }
        }

        protected async void onMemberAdd(FamilyMember familyMember)
        {
            var result = await MemberDataService.Create(new Domain.Commands.CreateMemberCommand()
            {
                Avatar = familyMember.avtar,
                FirstName = familyMember.firstname,
                LastName = familyMember.lastname,
                Email = familyMember.email,
                Roles = familyMember.role
            });

            if (result != null && result.Payload != null && result.Payload.Id != Guid.Empty)
            {
                members.Add(new FamilyMember()
                {
                    avtar = result.Payload.Avatar,
                    email = result.Payload.Email,
                    firstname = result.Payload.FirstName,
                    lastname = result.Payload.LastName,
                    role = result.Payload.Roles,
                    id = result.Payload.Id
                });

                leftMenuItem.Add(new MenuItem
                {
                    iconColor = result.Payload.Avatar,
                    label = result.Payload.FirstName,
                    referenceId = result.Payload.Id
                });


                showCreator = false;
                //makeMenuItemActive(leftMenuItem.FirstOrDefault());
                leftMenuItem.FirstOrDefault().InvokClickCallback(leftMenuItem.FirstOrDefault());
                StateHasChanged();
            }
        }
        protected void onDragStart(TaskModel task) => taskToAssign = task;

        protected async void onDrop(Guid memberId)
        {
            await assignTask(taskToAssign, memberId);
        }
        protected async Task assignTask(TaskModel task, Guid familyMemberId)
        {
            var response = await TaskManagerService.AssignTask(new Domain.Commands.UpdateTaskCommand()
            {
                AssignedMemberId = familyMemberId,
                Id = task.id,
                IsComplete = task.isDone,
                Subject = task.text
            
            });
            if (response.Succeed)
                allTasks.FirstOrDefault(t => t.id == task.id).member = members.FirstOrDefault(member => member.id == familyMemberId);
        
            StateHasChanged();
            
        }
        protected async void AddTask()
        {
            var response = await TaskManagerService.Create(new Domain.Commands.CreateTaskCommand()
            {
                Subject = NewTaskSubject,
                IsComplete = false
            });
            allTasks.Add(new TaskModel()
            {
                id = response.Payload.Id,
                isDone = response.Payload.IsComplete,
                text = response.Payload.Subject

            });
            tasksToShow = allTasks.ToArray();
            StateHasChanged();
        }
    }
}
