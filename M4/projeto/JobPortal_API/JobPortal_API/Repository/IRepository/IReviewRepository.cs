using JobPortal_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortal_API.Repository.IRepository
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();
        Task<Review> GetAsync(int id);
        Task<List<Review>> GetReviewsByEmpresaIdAsync(int empresaId);
        Task<Review> CreateAsync(Review entity);
        Task<Review> UpdateAsync(Review entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmpresaExistsAsync(int idEmpresa);
    }
}
