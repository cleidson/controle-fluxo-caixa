using ControleFluxoCaixa.Core.Logic.Interfaces.Repository;
using ControleFluxoCaixa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControleFluxoCaixa.Infrastructure.Respositories
{
    public class ReadOnlyDataRepository<T> : IReadOnlyDataRepository<T> where T : class
    {
        private readonly ControleFluxoCaixaReadOnlyDbContext _context; // <- Agora explícito
        private readonly DbSet<T> _dbSet;

        public ReadOnlyDataRepository(ControleFluxoCaixaReadOnlyDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

    }
}
