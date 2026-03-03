using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Common;
using TaskApp.Application.TodoTasks;
using TaskApp.Application.TodoTasks.Create;
using TaskApp.Application.TodoTasks.Delete;
using TaskApp.Application.TodoTasks.List;
using TaskApp.Application.TodoTasks.MoveToNextStatus;
using TaskApp.Application.TodoTasks.Update;

namespace TaskApp.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ICommandDispatcher cmdDispatcher, IQueryDispatcher queryDispatcher) : ControllerBase
    {
        
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<TodoTaskDto> Create([FromBody] CreateTodoTaskCommand command, CancellationToken cancellationToken)
        {
            return await cmdDispatcher.Send(command, cancellationToken);
        }

        [HttpPut("{id:guid}")]
        public async Task<TodoTaskDto> Update(Guid id, [FromBody] UpdateTodoTaskCommand command, CancellationToken cancellationToken)
        {
            return await cmdDispatcher.Send(command with { Id = id }, cancellationToken);
        }

        [HttpGet]
        [Authorize]
        public async Task<PagedResult<TodoTaskResponse>> Get([FromQuery] GetTodoTasksQuery query, CancellationToken cancellationToken)
        {
            return await queryDispatcher.Send(query, cancellationToken);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            return await cmdDispatcher.Send(new DeleteTodoTaskCommand(id), cancellationToken) ? NoContent() : NotFound();
        }

        [HttpPut("MoveToNextStatus/{id:guid}")]
        public async Task<TodoTaskDto> MpveToNextStatus(Guid id, CancellationToken cancellationToken)
        {
            return await cmdDispatcher.Send(new MoveToNextStatusCommand(id), cancellationToken);
        }
    }
}
