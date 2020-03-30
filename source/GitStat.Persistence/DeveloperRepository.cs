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

        //public IEnumerable<Developer> GetDevOpStats() => _dbContext
        //                                   .Developers
        //                                   .GroupBy(d => d.Name)
        //                                   .Select(d => new Developer
        //                                   {
        //                                       Name = d.Key,
        //                                       Changes = 
                                               
                                               

        //                                   })
        //                                   .OrderBy(d => d.)
        //                                   .ToArray();
    }
}