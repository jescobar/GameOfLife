using LifeApi.Client.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LifeApi.BusinessLogic.Managers;
public class GenericEntityManager<C, E>: GenericManager<C>
    where C: DbContext, new()
    where E: class, IAuditable, IIdentifiable
{
    public GenericEntityManager(C context): base(context)
    {
    }

    public virtual Task<E> Save<T>(T data, CancellationToken cancellationToken = default)
    where T: class, IIdentifiable
    {
        return base.Save<E, T>(data, cancellationToken);
    }

    public Task<E?> GetById(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return base.GetById<E>(id, cancellationToken);
    }

    public Task<E?> GetById(
        Guid id,
        Func<Guid, IQueryable<E>, IQueryable<E>> customQueryFn,
        CancellationToken cancellationToken = default
    )
    {
        return base.GetById<E>(id, customQueryFn, cancellationToken);
    }

    public Task<E?> GetById<T>(
        T record,
        CancellationToken cancellationToken = default
    )
    where T: class, IIdentifiable
    {
        return base.GetById<E, T>(record, cancellationToken);
    }

    public Task<E?> GetById<T>(
        T record,
        Func<Guid, IQueryable<E>, IQueryable<E>> customQueryFn,
        CancellationToken cancellationToken = default
    )
    where T: class, IIdentifiable
    {
        return base.GetById<E, T>(record, customQueryFn, cancellationToken);
    }
}