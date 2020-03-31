using System;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using GitStat.Persistence;
using System.Collections.Generic;

namespace GitStat.ImportConsole
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Import der Commits in die Datenbank");
            using (IUnitOfWork unitOfWorkImport = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWorkImport.DeleteDatabase();
                Console.WriteLine("Datenbank migrieren");
                unitOfWorkImport.MigrateDatabase();
                Console.WriteLine("Commits werden von commits.txt eingelesen");
                var commits = ImportController.ReadFromCsv();
                if (commits.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Commits eingelesen");
                    return;
                }
                Console.WriteLine(
                    $"  Es wurden {commits.Count()} Commits eingelesen, werden in Datenbank gespeichert ...");
                unitOfWorkImport.CommitRepository.AddRange(commits);
                int countDevelopers = commits.GroupBy(c => c.Developer).Count();
                int savedRows = unitOfWorkImport.SaveChanges();
                Console.WriteLine(
                    $"{countDevelopers} Developers und {savedRows - countDevelopers} Commits wurden in Datenbank gespeichert!");
                Console.WriteLine();
                var csvCommits = commits.Select(c =>
                    $"{c.Developer.Name};{c.Date};{c.Message};{c.HashCode};{c.FilesChanges};{c.Insertions};{c.Deletions}");
                File.WriteAllLines("commits.csv", csvCommits, Encoding.UTF8);
            }
            Console.WriteLine("Datenbankabfragen");
            Console.WriteLine("=================");
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                var commits = unitOfWork.CommitRepository.GetCommitsFromLast4Wekks();
                Console.WriteLine("Commits der letzten 4 Wochen");
                Console.WriteLine("----------------------------");
                WriteCommits(commits);
                Console.WriteLine();

                Console.WriteLine("Commit mit Id 4");
                Console.WriteLine("---------------");
                var commit = unitOfWork.CommitRepository.GetCommitById(4);
                Console.WriteLine(commit.ToString());
                Console.WriteLine();

                Console.WriteLine("Statistik der Commits der Developer");
                Console.WriteLine("-----------------------------------");
                var stats = unitOfWork.DeveloperRepository.GetDevOpStats();
                foreach (var item in stats)
                {
                    Console.WriteLine($"{item.Name,-20} {item.Commits,-15} {item.Changes,-15}" +
                    $"{item.Inserts,-15}{item.Deletes}");
                }
                Console.WriteLine();
            }
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        

        private static void WriteCommits(Commit[] commits)
        {
            Console.WriteLine($"{"Developer",-20}{"Date",-15}{"FileChanges",-15}{"Insertions",-15}{"Deletions"}");
            
            for (int i = 0; i < commits.Length; i++)
            {
                Console.WriteLine($"{commits[i].Developer,-20} {commits[i].Date.ToShortDateString(),-15} {commits[i].FilesChanges,-15}" +
                    $"{commits[i].Insertions,-15}{commits[i].Deletions}");
            }
        }
    }
}
