using SIUTeam.EnglishStudy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Repositories
{
    public interface IUserProgressRepository : IGenericRepository<UserProgress>
    {
        Task<UserProgress?> GetUserProgressAsync(Guid userId, Guid courseId);
        Task<IEnumerable<UserProgress>> GetUserProgressByUserIdAsync(Guid userId);
    }
}
