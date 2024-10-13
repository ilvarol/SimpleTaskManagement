using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = SimpleTaskManagement.Api.Domain.Models.Task;

namespace SimpleTaskManagement.Api.Application.Interfaces.Repositories;

public interface ITaskRepository : IGenericRepository<Task>
{
    public Task GetByIdWithDependencyList(int id);
}