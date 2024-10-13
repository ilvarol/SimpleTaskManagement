using SimpleTaskManagement.Api.Domain.Models;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace SimpleTaskManagement.Api.Application.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    int Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    void Delete(int id);

    IQueryable<TEntity> AsQueryable();

    IList<TEntity> GetAll();

    TEntity? GetById(int id);

    IList<TEntity> GetList(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
}
