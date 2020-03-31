using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace GitStat.Persistence
{
    public class DeveloperRepository : IDeveloperRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DeveloperRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (string Name, int Commits, int Changes, int Inserts, int Deletes)[] GetDevOpStats()
        {
            return _dbContext
                   .Developers
                   .Include(d => d.Commits)
                   .Select(d => new
                   {
                       Name = d.Name,
                       Commits = d.Commits.Count,
                       Changes = d.Commits.Sum(c => c.FilesChanges),
                       Inserts = d.Commits.Sum(c => c.Insertions),
                       Deletes = d.Commits.Sum(c => c.Deletions)
                   })
                   .OrderByDescending(d => d.Commits)
                   .AsEnumerable()
                   .Select(d => (d.Name, d.Commits, d.Changes, d.Inserts, d.Deletes))
                   .ToArray();
        }
    }
}