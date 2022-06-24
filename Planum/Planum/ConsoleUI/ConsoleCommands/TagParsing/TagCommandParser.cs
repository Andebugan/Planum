using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TagCommandParser
    {
        public bool Parse(ref List<string> filters, List<string> argsList,
            ref bool showDescription,
            ref bool showCategory)
        {
            FilterGetter filterGetter = new FilterGetter();

            string[] filterStrings =
            {
                "-f",
                "-nf",
                "-sr",
                "-nsr"
            };

            for (int i = 0; i < argsList.Count; i++)
            {
                if (argsList[i] == "-c" && !showCategory)
                    showCategory = true;
                else if (argsList[i] == "-d" && !showDescription)
                    showDescription = true;
                else if (argsList[i].Length > 3 && filterStrings.Any(x => x == (argsList[i].Substring(0, x.Length))))
                {
                    int filter_len = filters.Count;
                    string[] intFilters =
                    {
                            "-f-i",
                            "-nf-i",
                            "-sr-i",
                            "-nsr-i",
                        };

                    string[] stringFilters =
                    {
                            "-f-n",
                            "-nf-n",
                            "-sr-n",
                            "-nsr-n",
                            "-f-c",
                            "-nf-c",
                            "-nsr-c",
                            "-sr-c"
                        };

                    bool needBreak = false;

                    if (!intFilters.Any(x => x == argsList[i].Split('=')[0]) &&
                        !stringFilters.Any(x => x == argsList[i].Split('=')[0]))
                    {
                        return false;
                    }

                    foreach (string filter in intFilters)
                    {
                        int rc = filterGetter.TryGetIdFilter(filter, argsList[i], ref filters);
                        if (rc == -1)
                        {
                            return false;
                        }
                    }

                    foreach (string filter in stringFilters)
                    {
                        int rc = filterGetter.TryGetStringFilter(filter, ref argsList, ref filters, ref i);
                        if (rc == -1)
                        {
                            return false;
                        }
                        else if (rc == -2)
                        {
                            needBreak = true;
                            break;
                        }
                    }

                    if (filters.Count == filter_len)
                    {
                        return false;
                    }

                    if (needBreak)
                        break;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
