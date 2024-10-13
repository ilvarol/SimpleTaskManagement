using AutoMapper;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Api.Domain.Models;
using System.Linq.Expressions;

namespace SimpleTaskManagement.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly List<TEntity> _storage = new();
    private readonly IMapper mapper;
    protected readonly object _lock = new();

    protected Action<TEntity>? BeforeSaving;
    protected Action<TEntity>? BeforeDeleting;

    public GenericRepository(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public int Add(TEntity entity)
    {
        lock (_lock)
        {
            if (entity.Id != 0)
                entity.Id = 0;

            BeforeSaving?.Invoke(entity);

            entity.Id = _storage.Count > 0 ? GetNextId() : 1;

            _storage.Add(entity);

            return entity.Id;
        }
    }

    private int GetNextId()
    {
        return _storage.Count > 0 ? _storage.Max(e => e.Id) + 1 : 1;
    }

    public void Update(TEntity entity)
    {
        lock (_lock)
        {
            BeforeSaving?.Invoke(entity);

            var existingEntity = _storage.SingleOrDefault(e => e.Id == entity.Id);
            if (existingEntity != null)
                mapper.Map(entity, existingEntity);
        }
    }

    public void Delete(TEntity entity)
    {
        BeforeDeleting?.Invoke(entity);

        lock (_lock)
        {
            _storage.Remove(entity);
        }
    }

    public void Delete(int id)
    {
        TEntity? entity;

        lock (_lock)
        {
            entity = _storage.SingleOrDefault(e => e.Id == id);
        }

        if (entity != null)
            BeforeDeleting?.Invoke(entity);

        lock (_lock)
        {
            if (entity != null)
                _storage.Remove(entity);
        }
    }

    public IQueryable<TEntity> AsQueryable()
    {
        lock (_lock)
        {
            return _storage.AsQueryable();
        }
    }

    public IList<TEntity> GetAll()
    {
        lock (_lock)
        {
            return _storage.ToList();
        }
    }

    public TEntity? GetById(int id)
    {
        lock (_lock)
        {
            var entity = _storage.SingleOrDefault(e => e.Id == id);
            return entity;
        }
    }

    public IList<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
    {
        lock (_lock)
        {
            var query = _storage.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            return query.ToList();
        }
    }

    public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
    {
        lock (_lock)
        {
            return _storage.AsQueryable().Where(predicate);
        }
    }
}
