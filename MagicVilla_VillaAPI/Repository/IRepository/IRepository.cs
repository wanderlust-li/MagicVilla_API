﻿using System.Linq.Expressions;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository.IRepository;

public interface IRepository<T> where T: class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    
    Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
    
    Task CreateAsync(T entity);

    Task RemoveAsync(T entity);

    Task SaveAsync();
}