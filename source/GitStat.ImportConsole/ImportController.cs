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
            Dictionary<string, Developer> developers = new Dictionary<string, Developer>();
            string filePath = MyFile.GetFullNameInApplicationTree(Filename);
            string[] lines = File.ReadAllLines(filePath);
            List<Commit> commits = new List<Commit>();
            bool isHeaderFound = false;
            string parseString = "";
            int changes = 0;
            int inserts = 0;
            int deletes = 0;
            bool isCommitAddOn = false;

            foreach (var item in lines)
            {
                string[] parts = item.Split(',');

                if (parts.Length >= 4)
                {
                    parseString = ParseText(parts);
                    isHeaderFound = true;
                }

                if (item.Contains("file changed") || item.Contains("files changed"))
                {
                    GetFooterInformation(parts, out changes, out inserts, out deletes);
                    isCommitAddOn = true;
                }

                if (isHeaderFound)
                {
                    string[] data = parseString.Split(';');
                    string name = data[1];
                    Developer tmp;
                    Commit commit = new Commit
                    {
                        Date = Convert.ToDateTime(data[2]),
                        HashCode = data[0],
                        Message = data[3]
                    };

                    if (isCommitAddOn)
                    {
                        commit.FilesChanges = changes;
                        commit.Insertions = inserts;
                        commit.Deletions = deletes;
                    }

                    if (developers.TryGetValue(data[1], out tmp))
                    {
                        commit.Developer = tmp;
                        tmp.Commits.Add(commit);
                    }
                    else
                    {
                        Developer newDeveloper = new Developer
                        {
                            Name = name,
                        };
                        commit.Developer = newDeveloper;
                        newDeveloper.Commits.Add(commit);
                        developers.Add(name, newDeveloper);
                    }

                    commits.Add(commit);
                    parseString = "";
                    isHeaderFound = false;
                    isCommitAddOn = false;
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
            if (parts.Length == 3)
            {
                inserts = GetNumber(parts[1]);
                deletes = GetNumber(parts[2]);
            }
            else
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
        }

        private static string ParseText(string[] parts)
        {
            string parseString = "";
            for (int i = 0; i < parts.Length; i++)
            {
                if (i < parts.Length - 1)
                {
                    parseString += parts[i] + ';';
                }
                else
                {
                    parseString += parts[i];
                }
            }
            return parseString;
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
