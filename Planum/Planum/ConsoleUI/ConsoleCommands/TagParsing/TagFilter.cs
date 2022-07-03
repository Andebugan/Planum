using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;
using System.Linq;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TagFilter
    {
        public List<Tag> Filter(List<string> filters, List<Tag> tags, ref bool parseSuccessfull,
            ITaskManager _taskManager, ITagManager _tagManager)
        {
            List<Tag> filteredTags = tags;
            int oldCount = filteredTags.Count;
            // filter
            string fname;
            foreach (var filter in filters)
            {
                fname = "-f-c";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string category = filter.Substring(fname.Length);
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Category == category)
                        {
                            if (!tempList.Contains(tag))
                                tempList.Add(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    filteredTags = tempList;
                    continue;
                }

                fname = "-f-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Id == id)
                        {
                            if (!tempList.Contains(tag))
                                tempList.Add(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    filteredTags = tempList;
                    continue;
                }

                fname = "-f-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string name = filter.Substring(fname.Length);
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Name == name)
                        {
                            if (!tempList.Contains(tag))
                                tempList.Add(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    filteredTags = tempList;
                    continue;
                }

                if (!parseSuccessfull)
                    return tags;
            }

            if (filters.Any(x => x.Substring(0, 2) == "-f") && filteredTags.Count == oldCount)
            {
                parseSuccessfull = false;
                return tags;
            }

            oldCount = filteredTags.Count;

            // not filter
            foreach (var filter in filters)
            {
                fname = "-nf-c";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>(filteredTags);
                    string category = filter.Substring(fname.Length);
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Category == category)
                        {
                            if (tempList.Contains(tag))
                                tempList.Remove(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    filteredTags = tempList;
                    continue;
                }

                fname = "-nf-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>(filteredTags);
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Id == id)
                        {
                            if (tempList.Contains(tag))
                                tempList.Remove(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    filteredTags = tempList;
                    continue;
                }

                fname = "-nf-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>(filteredTags);
                    string name = filter.Substring(fname.Length);
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Name == name)
                        {
                            if (tempList.Contains(tag))
                                tempList.Remove(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    filteredTags = tempList;
                    continue;
                }

                if (!parseSuccessfull)
                    return tags;
            }

            if (filters.Any(x => x.Substring(0, 3) == "-nf") && filteredTags.Count == oldCount)
            {
                parseSuccessfull = false;
                return tags;
            }

            return filteredTags;
        }
    }
}
