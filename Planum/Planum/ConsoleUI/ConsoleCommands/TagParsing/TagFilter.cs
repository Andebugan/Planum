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
            foreach (var filter in filters)
            {
                if (filter.Length > 4 && filter.Substring(0, 4) == "-f-c")
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string category = filter.Substring(4);
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
                        parseSuccessfull = false;
                    filteredTags = tempList;
                }
                else if (filter.Length > 4 && filter.Substring(0, 4) == "-f-i")
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    int id = int.Parse(filter.Substring(4));
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
                    filteredTags = tempList;
                }
                else if (filter.Length > 4 && filter.Substring(0, 4) == "-f-n")
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string name = filter.Substring(4);
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
                        parseSuccessfull = false;
                    filteredTags = tempList;
                }

                if (!parseSuccessfull)
                    return tags;
            }

            // not filter
            foreach (var filter in filters)
            {
                if (filter.Length > 4 && filter.Substring(0, 4) == "-nf-c")
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string category = filter.Substring(4);
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
                        parseSuccessfull = false;
                    filteredTags = tempList;
                }
                else if (filter.Length > 4 && filter.Substring(0, 4) == "-nf-i")
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    int id = int.Parse(filter.Substring(4));
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
                    filteredTags = tempList;
                }
                else if (filter.Length > 4 && filter.Substring(0, 4) == "-nf-n")
                {
                    bool added = false;
                    List<Tag> tempList = new List<Tag>();
                    string name = filter.Substring(4);
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
                        parseSuccessfull = false;
                    filteredTags = tempList;
                }

                if (!parseSuccessfull)
                    return tags;
            }

            return filteredTags;
        }
    }
}
