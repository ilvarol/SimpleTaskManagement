using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Complete;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Delete;
using SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTaskDetail;
using SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTasks;
using SimpleTaskManagement.Common.Models.Commands;

namespace SimpleTaskManagement.Api.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        IMediator mediator;

        public TasksController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var result = await mediator.Send(new GetTaskDetailQuery(id));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var result = await mediator.Send(new GetTasksQuery());

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskCommand createTaskCommand)
        {
            var result = await mediator.Send(createTaskCommand);

            return CreatedAtAction(nameof(GetTask), new { id = result }, result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskCommand updateTaskCommand)
        {
            if (id > 0)
                updateTaskCommand.Id = id;

            var result = await mediator.Send(updateTaskCommand);

            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await mediator.Send(new DeleteTaskCommand(id));

            return Ok(result);
        }

        [HttpPatch]
        [Route("{id}/Complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var result = await mediator.Send(new CompleteTaskCommand(id));

            return Ok(result);
        }
    }
}
