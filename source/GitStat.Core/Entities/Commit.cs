using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitStat.Core.Entities
{
    public class Commit : EntityObject
    {

        public int DeveloperId { get; set; }

        [ForeignKey(nameof(DeveloperId))]
        public Developer Developer { get; set; }

        public DateTime Date { get; set; }
        public string HashCode { get; set; }

        public string Message { get; set; }

        public int FilesChanges { get; set; } = 0;
        public int Insertions { get; set; } = 0;
        public int Deletions { get; set; } = 0;

        public override string ToString()
        {
            return $"{Developer,-20}{Date.ToShortDateString(),-15}{FilesChanges,-15}{Insertions,-15}{Deletions}";
        }

    }
}
