using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TaskCommandParser
    {
        public bool Parse(ref List<string> filters, List<string> argsList,
        ref bool showDescription,
        ref bool showTags,
        ref bool showStatus,
        ref bool showParent,
        ref bool showChildren,
        ref bool showStatusQueue,
        ref bool showStartTime,
        ref bool showDeadline,
        ref bool showRepeatPeriod,
        ref bool showArchivedTasks,
        ref bool showOnlyArchivedTasks,
        ref bool showOverdueTasks,
        ref bool showTodayTasks,
        ref bool showNotOverdueTasks)
        {
            FilterGetter filterGetter = new FilterGetter();
            for (int i = 0; i < argsList.Count; i++)
            {
                if (argsList[i] == "-all")
                {
                    showDescription = true;
                    showTags = true;
                    showStatus = true;
                    showParent = true;
                    showChildren = true;
                    showStatusQueue = true;
                    showStartTime = true;
                    showDeadline = true;
                    showRepeatPeriod = true;
                }
                else if (argsList[i] == "-d" && !showDescription)
                    showDescription = true;
                else if (argsList[i] == "-t" && !showTags)
                    showTags = true;
                else if (argsList[i] == "-s" && !showStatus)
                    showStatus = true;
                else if (argsList[i] == "-p" && !showParent)
                    showParent = true;
                else if (argsList[i] == "-c" && !showChildren)
                    showChildren = true;
                else if (argsList[i] == "-sq" && !showStatusQueue)
                    showStatusQueue = true;
                else if (argsList[i] == "-st" && !showStartTime)
                    showStartTime = true;
                else if (argsList[i] == "-dl" && !showDeadline)
                    showDeadline = true;
                else if (argsList[i] == "-rp" && !showRepeatPeriod)
                    showRepeatPeriod = true;
                else if (argsList[i] == "-rp" && !showRepeatPeriod)
                    showRepeatPeriod = true;
                else if (argsList[i] == "-a" && !showArchivedTasks)
                    showArchivedTasks = true;
                else if (argsList[i] == "-ao" && !showOnlyArchivedTasks)
                    showOnlyArchivedTasks = true;
                else if (argsList[i] == "-od" && !showOverdueTasks)
                    showOverdueTasks = true;
                else if (argsList[i] == "-tt" && !showTodayTasks)
                    showTodayTasks = true;
                else if (argsList[i] == "-nod" && !showNotOverdueTasks)
                    showNotOverdueTasks = true;
                else if (argsList[i].Length > 3 && (argsList[i].Substring(0, 2) == "-f" || argsList[i].Substring(0, 3) == "-sr"))
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
            return true;
        }
    }
}
