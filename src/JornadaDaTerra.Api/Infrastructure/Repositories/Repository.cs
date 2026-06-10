using JornadaDaTerra.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JornadaDaTerra.Api.Infrastructure.Repositories;

/// <summary>Implementação genérica do <see cref="IRepository{T}"/> sobre o EF Core.</summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet.AsNoTracking();

        if (filter is not null)
            query = query.Where(filter);

        if (include is not null)
            query = include(query);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet;
        if (filter is not null)
            query = query.Where(filter);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(
        int id,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        CancellationToken cancellationToken = default)
    {
        // GetByIdAsync usa a PK "Id" por convenção do domínio.
        IQueryable<T> query = DbSet;
        if (include is not null)
            query = include(query);

        return await query.FirstOrDefaultAsync(
            e => EF.Property<int>(e, "Id") == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        // Usa COUNT(*) > 0 em vez de AnyAsync(): o Oracle (< 23c) não possui literais
        // booleanos em SQL, e o EF gera "THEN True ELSE False" ao projetar EXISTS,
        // resultando em ORA-00904. COUNT(*) é traduzido de forma compatível.
        => await DbSet.CountAsync(predicate, cancellationToken) > 0;

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);
}
