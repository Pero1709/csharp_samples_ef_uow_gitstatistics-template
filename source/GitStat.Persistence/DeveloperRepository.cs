using GitStat.Core.Contracts;
using GitStat.Core.Entities;

namespace GitStat.Persistence
{
    public class DeveloperRepository : IDeveloperRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DeveloperRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Commit[] GetDevOpStats()
        {
            throw new System.NotImplementedException();
        }
    }
}