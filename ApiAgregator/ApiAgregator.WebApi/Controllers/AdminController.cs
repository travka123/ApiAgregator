using ApiAgregator.Entities;
using ApiAgregator.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiAgregator.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        [Route("users/{userId}/tasks")]
        public IActionResult Tasks([FromRoute] int userId, [FromServices] ICronTaskRepository cronTaskRepository)
        {
            var tasks = cronTaskRepository.GetTasksByUserId(userId);

            return Ok(tasks.Select(task => new
            {
                id = task.Id,
                ownerId = task.OwnerId,
                name = task.Name,
                description = task.Description,
                apiName = task.ApiName,
                expression = task.Expression.ToString(),
                lastFire = task.LastFire,
                parameters = task.Parameters
            }));
        }

        [HttpGet]
        [Route("users/{userId}/tasks/{taskId}/calls")]
        public IActionResult Tasks([FromRoute] int userId, [FromRoute] int taskId, 
            [FromServices] ICronTaskRepository cronTaskRepository)
        {
            return Ok(cronTaskRepository.GetTaskCalls(taskId));
        }

        [Route("[action]")]
        public IActionResult Statistics([FromServices] IUserRepository userRepository)
        {
            var users = userRepository.GetUsers();
            var stats = userRepository.GetUsersStatistics().ToDictionary((u) => u.UserId);
            return Ok(users.Select(u => {
                var stat = stats[u.Id];
                return new
                {
                    username = u.Username,
                    email = u.Email,
                    totalTasks = stat.TotalTasks,
                    totalCalls = stat.TotalCalls,
                    totalErrorCalls = stat.TotalErrorCalls
                };
            }));
        }
    }
}
