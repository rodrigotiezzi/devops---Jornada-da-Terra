using System.Linq.Expressions;

namespace JornadaDaTerra.Api.Infrastructure.Repositories;

/// <summary>
/// Abstração genérica de acesso a dados. Mantém os services desacoplados do EF Core
/// e facilita testes (pode ser substituída por um fake/mocked em testes unitários).
/// </summary>
public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        int id,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
