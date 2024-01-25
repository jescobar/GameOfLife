using LifeApi.Client.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LifeApi.BusinessLogic.Managers;

public class GenericManager<C>
    where C : DbContext
{
    protected readonly C context;

    public GenericManager(C context)
    {
        this.context = context;
    }

    /// <summary>
    /// Saves a record (Upsert) in the database
    /// </summary>
    /// <typeparam name="E">The entity used to save the record</typeparam>
    /// <typeparam name="T">The Dto that can be converted into the entity to save</typeparam>
    /// <param name="data">Returns the entity that was saved</param>
    /// <returns></returns>
    public async Task<E> Save<E, T>(T data, CancellationToken cancellationToken = default)
    where E : class, IIdentifiable, IAuditable
    where T : IIdentifiable
    {
        DbSet<E> dbSet = GetDbSet<E>();
        E resultRecord;

        if (data.Id.HasValue && data.Id != default)
        {
            var existingRecord = await dbSet.SingleOrDefaultAsync(x => x.Id == data.Id, cancellationToken);

            if (existingRecord != default)
            {
                data.Adapt(existingRecord);
                existingRecord.UpdatedOn = DateTime.UtcNow;
                resultRecord = existingRecord;
            }
            else
            {
                throw new Exception("entity not found");
            }
        }
        else
        {
            data.Id = Guid.NewGuid();
            var newRecord = data.Adapt<E>();
            newRecord.CreatedOn = DateTime.UtcNow;
            dbSet.Add(newRecord);
            resultRecord = newRecord;
        }

        await context.SaveChangesAsync(cancellationToken);

        return resultRecord;
    }

    public async Task<E> GetOrCreate<E, T>(
        T input,
        CancellationToken cancellationToken = default
    )
    where E : class, IIdentifiable, IStatusTable, IAuditable
    where T : class, IStatusTable, IIdentifiable
    {
        DbSet<E> dbSet = GetDbSet<E>();
        var record = await dbSet.FirstOrDefaultAsync(
            x => x.Name == input.Name, cancellationToken
        );

        record ??= await Save<E, T>(input, cancellationToken);

        return record;
    }

    public async Task<E> GetOrCreateStatus<E, T>(
        T input,
        CancellationToken cancellationToken = default
    )
    where E : class, IIdentifiable, IStatusTable, IAuditable
    where T : class, IStatusTable, IIdentifiable
    {
        DbSet<E> dbSet = GetDbSet<E>();

        var record = input.Id.HasValue ? await dbSet.FindAsync(
            input.Id.Value, cancellationToken
        ) : default;

        record ??= !string.IsNullOrWhiteSpace(input.Name) ? await dbSet.FirstOrDefaultAsync(
            x => x.Name == input.Name, cancellationToken
        ) : default;

        if (record == default)
        {

            record = input.Adapt<E>();
            input.Id ??= Guid.NewGuid();
            record.CreatedOn = DateTime.UtcNow;
            record.UpdatedOn = DateTime.UtcNow;
            dbSet.Add(record);
            await context.SaveChangesAsync(cancellationToken);
        }

        return record;
    }

    public Task<E?> GetById<E>(
        Guid id,
        CancellationToken cancellationToken = default
    )
    where E : class, IIdentifiable, IAuditable
    {
        DbSet<E> dbSet = GetDbSet<E>();
        return dbSet.FirstOrDefaultAsync(
            x => x.Id == id, cancellationToken
        );
    }

    public Task<int> HardDeleteByCustom<E>(
        Func<IQueryable<E>, IQueryable<E>> customQueryFn,
        CancellationToken cancellationToken = default
    )
    where E : class, IIdentifiable, IAuditable
    {
        DbSet<E> dbSet = GetDbSet<E>();

        var entitiesToDelete = customQueryFn(dbSet).ToList();

        dbSet.RemoveRange(entitiesToDelete);

        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<E?> GetById<E>(
        Guid id,
        Func<Guid, IQueryable<E>, IQueryable<E>> customQueryFn,
        CancellationToken cancellationToken = default
    )
    where E : class, IIdentifiable, IAuditable
    {
        DbSet<E> dbSet = GetDbSet<E>();
        var query = customQueryFn(id, dbSet.Where(x => x.Id == id));

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<E?> GetById<E, T>(
        T record,
        CancellationToken cancellationToken = default
    )
    where E : class, IIdentifiable, IAuditable
    where T : class, IIdentifiable
    {
        if (record.Id.HasValue)
        {
            return GetById<E>(record.Id.Value, cancellationToken);
        }
        return Task.FromResult<E?>(null);
    }

    public Task<E?> GetById<E, T>(
        T record,
        Func<Guid, IQueryable<E>, IQueryable<E>> customQueryFn,
        CancellationToken cancellationToken = default
    )
    where E : class, IIdentifiable, IAuditable
    where T : class, IIdentifiable
    {
        if (record.Id.HasValue)
        {
            return GetById(record.Id.Value, customQueryFn, cancellationToken);
        }
        return Task.FromResult<E?>(null);

    }

    protected async Task<TEntity> MapInputToRecord<TEntity, TInput>(TInput input, CancellationToken cancellationToken)
    where TInput : IIdentifiable
    where TEntity : class, IAuditable, IIdentifiable
    {
        var isNew = false;
        var dbSet = context.Set<TEntity>();
        TEntity record;

        if (input.Id.HasValue)
        {
            var existingRecord = await dbSet
                .SingleOrDefaultAsync(
                    x => x.Id == input.Id.Value,
                    cancellationToken
                );

            if (existingRecord != null)
            {
                record = existingRecord;
                input.Adapt(record);
                record.UpdatedOn = DateTime.UtcNow;
                return record;
            }
            else
            {
                isNew = true;
            }
        }

        if (isNew)
        {
            input.Id ??= Guid.NewGuid();
            record = input.Adapt<TEntity>();
            record.CreatedOn = DateTime.UtcNow;
            record.UpdatedOn = DateTime.UtcNow;
            dbSet.Add(record);

            return record;
        }
        else
        {
            throw new Exception("Error trying to convert the  input to the record");
        }
    }

    protected DbSet<E> GetDbSet<E>()
        where E : class, IAuditable, IIdentifiable
    {
        var dbSet = context.Set<E>() ?? throw new Exception("Could not find the specified DbSet");
        return dbSet;
    }

}