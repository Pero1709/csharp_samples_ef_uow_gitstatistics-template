using GitStat.Core.Entities;

namespace GitStat.Core.Contracts
{
    public interface IDeveloperRepository
    {
        public (string Name, int Commits, int Changes, int Inserts, int Deletes)[] GetDevOpStats();
    }
}
