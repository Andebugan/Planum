using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TagFilter
    {
        public List<Tag> Filter(List<string> filters, List<Tag> tags, ref bool parseSuccessfull,
            ITaskManager _taskManager, ITagManager _tagManager)
        {
            List<Tag> filteredTags = tags;
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

            // not filter
            foreach (var filter in filters)
            {
                fname = "-nf-c";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string category = filter.Substring(fname.Length);
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Category != category)
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

                fname = "-nf-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Id != id)
                        {
                            if (!tempList.Contains(tag))
                                tempList.Add(tag);
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

                fname = "-nf-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string name = filter.Substring(fname.Length);
                    foreach (var tag in filteredTags)
                    {
                        if (tag.Name != name)
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

            return filteredTags;
        }
    }
}
