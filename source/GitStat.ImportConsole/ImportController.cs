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
            string parseString = "";
            List<Commit> result = new List<Commit>();
            bool isHeaderFound = false;
            bool isFotterFound = false;

            foreach (var item in lines)
            {
                string[] parts = item.Split(',');

                if (parts.Length >= 4 && !isHeaderFound)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (i < 4)
                        {
                            parseString += parts[i] + ';';
                        }
                    }
                    isHeaderFound = true;
                }
                if (item.Contains("files") && isHeaderFound)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {

                        if (parts.Length - 1 != i)
                        {
                            parseString += parts[i] + ';';
                        }
                        else
                        {
                            parseString += parts[i];
                        }
                    }
                    isFotterFound = true;
                }

                if (isHeaderFound && isFotterFound)
                {
                    string[] data = parseString.Split(';');
                    Commit commit = new Commit
                    {
                        Date = Convert.ToDateTime(data[2]),
                        HashCode = data[0],
                        Message = data[3],
                        FilesChanges = GetNumber(data[4]),
                        Insertions = GetNumber(data[5]),
                        Deletions = GetNumber(data[6])
                    };

                    Developer tmp;
                    if (!developers.TryGetValue(data[1], out tmp))
                    {
                        Developer newDeveloper = new Developer
                        {
                            Name = data[1],
                        };
                        commit.Developer = newDeveloper;
                        newDeveloper.Commits.Add(commit);
                        developers.Add(data[1], newDeveloper);
                    }
                    else
                    {
                        commit.Developer = tmp;
                        tmp.Commits.Add(commit);
                    }
                    isFotterFound = false;
                    isHeaderFound = false;
                    parseString = "";
                    result.Add(commit);
                }
            }
            return result.ToArray();
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
                number = "0";
            }
            return Convert.ToInt32(number);
        }
    }
}
