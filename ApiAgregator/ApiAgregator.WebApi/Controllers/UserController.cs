using ApiAgregator.Entities;
using ApiAgregator.Services.APIs;
using ApiAgregator.Services.Repositories;
using ApiAgregator.WebApi.Models.Request;
using ApiAgregator.WebApi.Models.Response;
using ApiAgregator.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiAgregator.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Policy = "user")]
public class UserController : ControllerBase
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult Tasks([FromServices] ICronTaskRepository cronTaskRepository)
    {
        var tasks = cronTaskRepository.GetTasksByUserId(ClaimsParser.GetUserId(User));
        return Ok(tasks.Select(task => new {
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

    [HttpPost]
    [Route("[action]")]
    public IActionResult Tasks([FromBody] UserTaskAddRequest request,
        [FromServices] ExternalApisService externalApisService)
    {
        if (!CronExpression.TryParse(request.Expression, out var cronExpression))
        {
            return BadRequest("invalid cron expression");
        }

        try
        {
            var task = externalApisService.AddTask(ClaimsParser.GetUserId(User), request.ApiName, request.Name,
                request.Description, cronExpression!, request.Parameters);

            return Ok(new { id = task.Id, ownerId = task.OwnerId, name = task.Name, description = task.Description,
                apiName = task.ApiName, expression = task.Expression.ToString(), lastFire = task.LastFire, 
                parameters = task.Parameters });
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("[action]/{taskId}")]
    public IActionResult Tasks([FromRoute] int taskId, [FromBody] UserTaskUpdateRequest request,
        [FromServices] ExternalApisService externalApisService)
    {
        if (!CronExpression.TryParse(request.Expression, out var cronExpression))
        {
            return BadRequest("invalid cron expression");
        }

        try
        {
            var task = externalApisService.UpdateTask(taskId, ClaimsParser.GetUserId(User), request.ApiName, request.Name,
                request.Description, cronExpression!, request.Parameters);

            return Ok(new { id = task.Id, ownerId = task.OwnerId, name = task.Name, description = task.Description,
                apiName = task.ApiName, expression = task.Expression.ToString(), lastFire = task.LastFire, 
                parameters = task.Parameters });
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpDelete]
    [Route("tasks/{taskId}")]
    public IActionResult TaskDelete([FromRoute] int taskId, [FromServices] ExternalApisService externalApisService)
    {
        try
        {
            externalApisService.DeleteTasks(taskId, ClaimsParser.GetUserId(User));
        }
        catch
        {
            return NotFound();
        }
        return Ok();
    }

    [HttpGet]
    [Route("apis")]
    public IActionResult GetApis([FromServices] ExternalApisService externalApisService)
    {
        return Ok(externalApisService.GetApis());
    }

    [HttpGet]
    [Route("apis/{apiName}")]
    public IActionResult GetForm([FromRoute] string apiName, [FromServices] ExternalApisService externalApisService)
    {
        var formBuilder = new FormBuilder();
        try
        {
            externalApisService.BuildForm(apiName, formBuilder);
        }
        catch
        {
            return BadRequest();
        }

        return Ok(formBuilder.From);
    }
}
