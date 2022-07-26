using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TaskCommandParser
    {
        public bool Parse(ref List<string> filters, List<string> argsList, ref Dictionary<string,
            bool> boolParams, ref Dictionary<string, string> stringParams, string command)
        {
            FilterGetter filterGetter = new FilterGetter();
            string[] filterStrings =
            {
                "-f",
                "-nf",
                "-sr",
                "-nsr",
                "-dp",
                "-"
            };

            if (argsList.Any(x => x == "-cr"))
            {
                boolParams["calendar"] = true;
                if (argsList.Count(x => x == "-cr") > 1)
                    return false;
                argsList.Remove("-cr");
            }
            else
                boolParams["list"] = true;
           

            for (int i = 0; i < argsList.Count; i++)
            {
                if (command == "show")
                {
                    if (argsList[i] == "-all" && boolParams["list"])
                    {
                        boolParams["showDescription"] = true;
                        boolParams["showTags"] = true;
                        boolParams["showStatus"] = true;
                        boolParams["showParent"] = true;
                        boolParams["showChildren"] = true;
                        boolParams["showStatusQueue"] = true;
                        boolParams["showStartTime"] = true;
                        boolParams["showDeadline"] = true;
                        boolParams["showRepeatPeriod"] = true;
                    }
                    else if (argsList[i] == "-d" && !boolParams["showDescription"] && boolParams["list"])
                        boolParams["showDescription"] = true;
                    else if (argsList[i] == "-t" && !boolParams["showTags"] && boolParams["list"])
                        boolParams["showTags"] = true;
                    else if (argsList[i] == "-s" && !boolParams["showStatus"] && boolParams["list"])
                        boolParams["showStatus"] = true;
                    else if (argsList[i] == "-p" && !boolParams["showParent"] && boolParams["list"])
                        boolParams["showParent"] = true;
                    else if (argsList[i] == "-c" && !boolParams["showChildren"] && boolParams["list"])
                        boolParams["showChildren"] = true;
                    else if (argsList[i] == "-sq" && !boolParams["showStatusQueue"] && boolParams["list"])
                        boolParams["showStatusQueue"] = true;
                    else if (argsList[i] == "-st" && !boolParams["showStartTime"] && boolParams["list"])
                        boolParams["showStartTime"] = true;
                    else if (argsList[i] == "-dl" && !boolParams["showDeadline"] && boolParams["list"])
                        boolParams["showDeadline"] = true;
                    else if (argsList[i] == "-rp" && !boolParams["showRepeatPeriod"] && boolParams["list"])
                        boolParams["showRepeatPeriod"] = true;
                    else if (argsList[i] == "-a" && !boolParams["showArchivedTasks"] && boolParams["list"])
                        boolParams["showArchivedTasks"] = true;
                    else if (argsList[i] == "-ao" && !boolParams["showOnlyArchivedTasks"] && boolParams["list"])
                        boolParams["showOnlyArchivedTasks"] = true;
                    else if (argsList[i] == "-m" && boolParams["calendar"])
                        stringParams["displayType"] = "m";
                    else if (argsList[i] == "-w" && boolParams["calendar"])
                        stringParams["displayType"] = "w";
                    else if (argsList[i] == "-y" && boolParams["calendar"])
                        stringParams["displayType"] = "y";
                    else if (argsList[i].Contains("-dp-") && boolParams["calendar"])
                        stringParams["displayPeriod"] = argsList[i].Substring(4);
                    else if (argsList[i] == "-od" && !boolParams["showOverdueTasks"])
                        boolParams["showOverdueTasks"] = true;
                    else if (argsList[i] == "-tt" && !boolParams["showTodayTasks"])
                        boolParams["showTodayTasks"] = true;
                    else if (argsList[i] == "-np" && !boolParams["showNoParent"])
                        boolParams["showNoParent"] = true;
                    else if (argsList[i] == "-nc" && !boolParams["showNoChildren"])
                        boolParams["showNoChildren"] = true;
                    else if (argsList[i] == "-nsq" && !boolParams["showNoStatuses"])
                        boolParams["showNoStatuses"] = true;
                    else if (argsList[i] == "-nt" && !boolParams["showNoTags"])
                        boolParams["showNoTags"] = true;
                    else if (argsList[i] == "-ct" && !boolParams["showCurrentTasks"])
                        boolParams["showCurrentTasks"] = true;
                    else if (argsList[i] == "-nct" && !boolParams["showNotCurrentTasks"])
                        boolParams["showNotCurrentTasks"] = true;
                    else if (argsList[i].Length > 3 && filterStrings.Any(x => x == (argsList[i].Substring(0, x.Length))))
                    {
                        string[] intFilters =
                        {
                            "-f-i",
                            "-f-csi",
                            "-f-ti",
                            "-f-pi",
                            "-f-ci",
                            "-sr-i",
                            "-sr-csi",
                            "-sr-ti",
                            "-sr-pi",
                            "-sr-ci",

                            "-nf-i",
                            "-nf-csi",
                            "-nf-ti",
                            "-nf-pi",
                            "-nf-ci",
                            "-nsr-i",
                            "-nsr-csi",
                            "-nsr-ti",
                            "-nsr-pi",
                            "-nsr-ci"
                        };

                        string[] stringFilters =
                        {
                            "-f-n",
                            "-f-csn",
                            "-f-tn",
                            "-f-pn",
                            "-f-cn",
                            "-sr-n",
                            "-sr-csn",
                            "-sr-tn",
                            "-sr-pn",
                            "-sr-cn",

                            "-nf-n",
                            "-nf-csn",
                            "-nf-tn",
                            "-nf-pn",
                            "-nf-cn",
                            "-nsr-n",
                            "-nsr-csn",
                            "-nsr-tn",
                            "-nsr-pn",
                            "-nsr-cn"
                        };

                        if (!intFilters.Any(x => x == argsList[i].Split('=')[0]) &&
                            !stringFilters.Any(x => x == argsList[i].Split('=')[0]))
                        {
                            return false;
                        }

                        bool needBreak = false;
                        foreach (string filter in intFilters)
                        {
                            int rc = filterGetter.TryGetIdFilter(filter, argsList[i], ref filters);
                            if (rc == -1)
                            {
                                return false;
                            }
                        }

                        if (needBreak)
                            break;

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

                        if (needBreak)
                            break;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (command == "update")
                {
                    if (argsList[i] == "-n" && !boolParams["updateName"])
                        boolParams["updateName"] = true;
                    else if (argsList[i] == "-d" && !boolParams["updateDescription"])
                        boolParams["updateDescription"] = true;
                    else if (argsList[i] == "-t" && !boolParams["updateTags"])
                        boolParams["updateTags"] = true;
                    else if (argsList[i] == "-s" && !boolParams["updateStatus"])
                        boolParams["updateStatus"] = true;
                    else if (argsList[i] == "-p" && !boolParams["updateParent"])
                        boolParams["updateParent"] = true;
                    else if (argsList[i] == "-c" && !boolParams["updateChild"])
                        boolParams["updateChild"] = true;
                    else if (argsList[i] == "-st" && !boolParams["updateStartTime"])
                        boolParams["updateStartTime"] = true;
                    else if (argsList[i] == "-dl" && !boolParams["updateDeadline"])
                        boolParams["updateDeadline"] = true;
                    else if (argsList[i] == "-r" && !boolParams["updateRepeatPeriod"])
                        boolParams["updateRepeatPeriod"] = true;
                    else if (argsList[i] == "-tm" && !boolParams["updateTimed"])
                        boolParams["updateTimed"] = true;
                    else if (argsList[i].Length > 3 && filterStrings.Any(x => x == (argsList[i].Substring(0, x.Length))))
                    {
                        string[] intFilters =
                        {
                            "-i",
                        };

                        string[] stringFilters =
                        {
                           
                        };

                        if (!intFilters.Any(x => x == argsList[i].Split('=')[0]) &&
                            !stringFilters.Any(x => x == argsList[i].Split('=')[0]))
                        {
                            return false;
                        }

                        bool needBreak = false;
                        foreach (string filter in intFilters)
                        {
                            int rc = filterGetter.TryGetIdFilter(filter, argsList[i], ref filters);
                            if (rc == -1)
                            {
                                return false;
                            }
                            if (filters.FindAll(x => x.Substring(0, 2) == "-i").Count > 1)
                                return false;
                        }

                        if (needBreak)
                            break;

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

                        if (needBreak)
                            break;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (command == "archive")
                {
                   if (argsList[i].Length > 3 && filterStrings.Any(x => x == (argsList[i].Substring(0, x.Length))))
                    {
                        string[] intFilters =
                        {
                            "-f-i",
                            "-f-csi",
                            "-f-ti",
                            "-f-pi",
                            "-f-ci",
                            "-sr-i",
                            "-sr-csi",
                            "-sr-ti",
                            "-sr-pi",
                            "-sr-ci",

                            "-nf-i",
                            "-nf-csi",
                            "-nf-ti",
                            "-nf-pi",
                            "-nf-ci",
                            "-nsr-i",
                            "-nsr-csi",
                            "-nsr-ti",
                            "-nsr-pi",
                            "-nsr-ci"
                        };

                        string[] stringFilters =
                        {
                            "-f-n",
                            "-f-csn",
                            "-f-tn",
                            "-f-pn",
                            "-f-cn",
                            "-sr-n",
                            "-sr-csn",
                            "-sr-tn",
                            "-sr-pn",
                            "-sr-cn",

                            "-nf-n",
                            "-nf-csn",
                            "-nf-tn",
                            "-nf-pn",
                            "-nf-cn",
                            "-nsr-n",
                            "-nsr-csn",
                            "-nsr-tn",
                            "-nsr-pn",
                            "-nsr-cn"
                        };

                        if (!intFilters.Any(x => x == argsList[i].Split('=')[0]) &&
                            !stringFilters.Any(x => x == argsList[i].Split('=')[0]))
                        {
                            return false;
                        }

                        bool needBreak = false;
                        foreach (string filter in intFilters)
                        {
                            int rc = filterGetter.TryGetIdFilter(filter, argsList[i], ref filters);
                            if (rc == -1)
                            {
                                return false;
                            }
                        }

                        if (needBreak)
                            break;

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

                        if (needBreak)
                            break;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (command == "unarchive")
                {
                    if (argsList[i].Length > 3 && filterStrings.Any(x => x == (argsList[i].Substring(0, x.Length))))
                    {
                        string[] intFilters =
                        {
                            "-f-i",
                            "-f-csi",
                            "-f-ti",
                            "-f-pi",
                            "-f-ci",
                            "-sr-i",
                            "-sr-csi",
                            "-sr-ti",
                            "-sr-pi",
                            "-sr-ci",

                            "-nf-i",
                            "-nf-csi",
                            "-nf-ti",
                            "-nf-pi",
                            "-nf-ci",
                            "-nsr-i",
                            "-nsr-csi",
                            "-nsr-ti",
                            "-nsr-pi",
                            "-nsr-ci"
                        };

                        string[] stringFilters =
                        {
                            "-f-n",
                            "-f-csn",
                            "-f-tn",
                            "-f-pn",
                            "-f-cn",
                            "-sr-n",
                            "-sr-csn",
                            "-sr-tn",
                            "-sr-pn",
                            "-sr-cn",

                            "-nf-n",
                            "-nf-csn",
                            "-nf-tn",
                            "-nf-pn",
                            "-nf-cn",
                            "-nsr-n",
                            "-nsr-csn",
                            "-nsr-tn",
                            "-nsr-pn",
                            "-nsr-cn"
                        };

                        if (!intFilters.Any(x => x == argsList[i].Split('=')[0]) &&
                            !stringFilters.Any(x => x == argsList[i].Split('=')[0]))
                        {
                            return false;
                        }

                        bool needBreak = false;
                        foreach (string filter in intFilters)
                        {
                            int rc = filterGetter.TryGetIdFilter(filter, argsList[i], ref filters);
                            if (rc == -1)
                            {
                                return false;
                            }
                        }

                        if (needBreak)
                            break;

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

                        if (needBreak)
                            break;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (command == "complete")
                {
                    if (argsList[i].Length > 3 && filterStrings.Any(x => x == (argsList[i].Substring(0, x.Length))))
                    {
                        string[] intFilters =
                        {
                            "-f-i",
                            "-f-csi",
                            "-f-ti",
                            "-f-pi",
                            "-f-ci",
                            "-sr-i",
                            "-sr-csi",
                            "-sr-ti",
                            "-sr-pi",
                            "-sr-ci",

                            "-nf-i",
                            "-nf-csi",
                            "-nf-ti",
                            "-nf-pi",
                            "-nf-ci",
                            "-nsr-i",
                            "-nsr-csi",
                            "-nsr-ti",
                            "-nsr-pi",
                            "-nsr-ci"
                        };

                        string[] stringFilters =
                        {
                            "-f-n",
                            "-f-csn",
                            "-f-tn",
                            "-f-pn",
                            "-f-cn",
                            "-sr-n",
                            "-sr-csn",
                            "-sr-tn",
                            "-sr-pn",
                            "-sr-cn",

                            "-nf-n",
                            "-nf-csn",
                            "-nf-tn",
                            "-nf-pn",
                            "-nf-cn",
                            "-nsr-n",
                            "-nsr-csn",
                            "-nsr-tn",
                            "-nsr-pn",
                            "-nsr-cn"
                        };

                        if (!intFilters.Any(x => x == argsList[i].Split('=')[0]) &&
                            !stringFilters.Any(x => x == argsList[i].Split('=')[0]))
                        {
                            return false;
                        }

                        bool needBreak = false;
                        foreach (string filter in intFilters)
                        {
                            int rc = filterGetter.TryGetIdFilter(filter, argsList[i], ref filters);
                            if (rc == -1)
                            {
                                return false;
                            }
                        }

                        if (needBreak)
                            break;

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

                        if (needBreak)
                            break;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
