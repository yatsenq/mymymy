// <copyright file="Repository.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Context;
using StayFit.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StayFit.DAL.Repositories;

/// <summary>
/// Базова реалізація загального репозиторію для доступу до даних.
/// </summary>
/// <typeparam name="T">Тип сутності.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly StayFitDbContext context;
    private readonly DbSet<T> dbSet;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="Repository{T}"/>.
    /// </summary>
    /// <param name="context">Контекст бази даних.</param>
    public Repository(StayFitDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<T>();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> GetAllAsync() => await this.dbSet.ToListAsync();

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(int id) => await this.dbSet.FindAsync(id);

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await this.dbSet.Where(predicate).ToListAsync();

    /// <inheritdoc/>
    public async Task AddAsync(T entity)
    {
        await this.dbSet.AddAsync(entity);
        await this.context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(T entity)
    {
        this.dbSet.Update(entity);
        await this.context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var entity = await this.dbSet.FindAsync(id);
        if (entity != null)
        {
            this.dbSet.Remove(entity);
            await this.context.SaveChangesAsync();
        }
    }
}