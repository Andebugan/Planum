using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class FilterGetter
    {
        public int TryGetIdFilter(string filterName, string parsedString, ref List<string> filters)
        {
            string filter = filterName + "=";
            if (parsedString.Length > filter.Length && parsedString.Substring(0, filter.Length) == filter)
            {
                int id;
                if (!int.TryParse(parsedString.Substring(filter.Length), out id) || id < 0)
                {
                    return -1;
                }
                filters.Add(filterName + id.ToString());
                return 0;
            }
            return 1;
        }

        public int TryGetStringFilter(string filterName, ref List<string> argsList, ref List<string> filters, ref int i)
        {
            string filter = filterName + "=";
            if (argsList[i].Length > filter.Length && argsList[i].Substring(0, filter.Length) == filter)
            {
                string filterValue = argsList[i].Substring(filter.Length);

                if (filterValue[0] == '{' && filterValue[filterValue.Length - 1] == '}')
                {
                    filterValue = filterValue.Replace("{", "");
                    filterValue = filterValue.Replace("}", "");
                    filters.Add(filterName + filterValue);
                }
                else if (filterValue[0] == '{' && filterValue[filterValue.Length - 1] != '}')
                {
                    i += 1;
                    while (i < argsList.Count)
                    {
                        filterValue += " " + argsList[i];
                        if (argsList[i][argsList[i].Length - 1] == '}')
                        {
                            i += 1;
                            break;
                        }
                        i += 1;
                    }

                    filterValue = filterValue.Replace("{", "");
                    filterValue = filterValue.Replace("}", "");
                    filters.Add(filterName + filterValue);

                    if (i == argsList.Count)
                        return -2;
                    else
                        i -= 1;
                }
                else
                {
                    filters.Add(filterName + filterValue);
                }
                return 0;
            }
            return 1;
        }
    }
}
