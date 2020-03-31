using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
using Utils;

namespace GitStat.ImportConsole
{
    public class ImportController
    {
        const string Filename = "commits.txt";

        /// <summary>
        /// Liefert die Messwerte mit den dazugehörigen Sensoren
        /// </summary>
        public static Commit[] ReadFromCsv()
        {
            string filePath = MyFile.GetFullNameInApplicationTree(Filename);
            string[] lines = File.ReadAllLines(filePath);
            Dictionary<string, Developer> developers = new Dictionary<string, Developer>();
            List<Commit> commits = new List<Commit>();
            Commit commit = null;
            bool isHeaderFound = false;
            int changes = 0;
            int inserts = 0;
            int deletes = 0;

            foreach (var item in lines)
            {
                string[] parts = item.Split(',');

                if (parts.Length >= 4)
                {
                    if (isHeaderFound)
                    {
                        commits.Add(commit);
                        isHeaderFound = false;
                    }

                    string hashCode = parts[0];
                    string name = parts[1];
                    string message = parts[3];
                    DateTime dateTime = Convert.ToDateTime(parts[2]);

                    commit = new Commit
                    {
                        Date = dateTime,
                        HashCode = hashCode,
                        Message = message
                    };

                    Developer devOp;
                    if (developers.TryGetValue(name, out devOp))
                    {
                        commit.Developer = devOp;
                        devOp.Commits.Add(commit);
                    }
                    else
                    {
                        Developer newDevOp = new Developer
                        {
                            Name = name,
                        };
                        commit.Developer = newDevOp;
                        newDevOp.Commits.Add(commit);
                        developers.Add(name, newDevOp);
                    }
                    isHeaderFound = true;
                }

                if (parts.Length > 1 && parts.Length < 4)
                {
                    GetFooterInformation(parts, out changes, out inserts, out deletes);
                    commit.FilesChanges = changes;
                    commit.Insertions = inserts;
                    commit.Deletions = deletes;
                    commits.Add(commit);
                    isHeaderFound = false;
                }
            }
            return commits.ToArray();
        }

        #region Private

        private static void GetFooterInformation(string[] parts, out int changes, out int inserts, out int deletes)
        {
            inserts = 0;
            deletes = 0;
            changes = GetNumber(parts[0]);

            if (parts.Length < 3)
            {
                if (parts[1].Contains("insertions"))
                {
                    inserts = GetNumber(parts[1]);
                }
                else if (parts[1].Contains("deletions"))
                {
                    deletes = GetNumber(parts[1]);
                }
            }
            else
            {
                inserts = GetNumber(parts[1]);
                deletes = GetNumber(parts[2]);
            }
        }

        private static int GetNumber(string expression)
        {
            string number = "";
            for (int i = 0; i < expression.Length; i++)
            {
                if (char.IsDigit(expression[i]))
                {
                    number += expression[i];
                }
            }

            if (string.IsNullOrEmpty(number))
            {
                throw new ArgumentNullException(nameof(expression));
            }
            return Convert.ToInt32(number);
        }

        #endregion
    }
}
