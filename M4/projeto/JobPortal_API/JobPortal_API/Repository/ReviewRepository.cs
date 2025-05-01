using JobPortal_API.Data;
using JobPortal_API.Models;
using JobPortal_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal_API.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _db;

        public ReviewRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Review> CreateAsync(Review entity)
        {
            // Verificar se a empresa existe
            var empresaExists = await _db.Empresa.AnyAsync(e => e.IdEmpresa == entity.IdEmpresa);
            if (!empresaExists)
            {
                throw new InvalidOperationException("Empresa com o Id fornecido não existe.");
            }

            await _db.Review.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Review.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            _db.Review.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Review.AnyAsync(e => e.IdReview == id);
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _db.Review
                .Include(r => r.Empresa)
                .ToListAsync();
        }

        public async Task<Review> GetAsync(int id)
        {
            return await _db.Review
                .Include(r => r.Empresa)
                .FirstOrDefaultAsync(r => r.IdReview == id);
        }

        public async Task<List<Review>> GetReviewsByEmpresaIdAsync(int empresaId)
        {
            return await _db.Review
                .Where(r => r.IdEmpresa == empresaId)
                .Include(r => r.Empresa)
                .ToListAsync();
        }

        public async Task<Review> UpdateAsync(Review entity)
        {
            // Verificar se a empresa existe
            var empresaExists = await _db.Empresa.AnyAsync(e => e.IdEmpresa == entity.IdEmpresa);
            if (!empresaExists)
            {
                throw new InvalidOperationException("Empresa com o Id fornecido não existe.");
            }

            _db.Review.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> EmpresaExistsAsync(int idEmpresa)
        {
            return await _db.Empresa.AnyAsync(e => e.IdEmpresa == idEmpresa);
        }
    }
}
