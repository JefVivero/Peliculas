using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPeliculas.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string url);
        Task<T> GetAsync(string url, int id);
        Task<bool> CreateAsync(string url, T item);
        Task<bool> UpdateAsync(string url, T item);
        Task<bool> DeleteAsync(string url, int id);
    }
}
