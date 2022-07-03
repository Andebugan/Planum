using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;
using System.Linq;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TagSelector
    {
        public List<Tag> Select(List<string> filters, List<Tag> tags, ref bool parseSuccessfull,
            ITaskManager _taskManager, ITagManager _tagManager)
        {
            List<Tag> selectedTags = new List<Tag>();
            string fname;
            // selector
            foreach (var filter in filters)
            {
                fname = "-sr-c";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string category = filter.Substring(fname.Length);
                    foreach (var tag in tags)
                    {
                        if (tag.Category == category)
                        {
                            if (!selectedTags.Contains(tag))
                                selectedTags.Add(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var tag in tags)
                    {
                        if (tag.Id == id)
                        {
                            if (!selectedTags.Contains(tag))
                                selectedTags.Add(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string name = filter.Substring(fname.Length);
                    foreach (var tag in tags)
                    {
                        if (tag.Name == name)
                        {
                            if (!selectedTags.Contains(tag))
                                selectedTags.Add(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                if (!parseSuccessfull)
                    return tags;
            }

            if (selectedTags.Count == 0 && filters.Any(x => x.Substring(0, 3) == "-sr"))
            {
                parseSuccessfull = false;
                return tags;
            }
            else if (selectedTags.Count == 0)
            {
                selectedTags = new List<Tag>(tags);
            }

            int oldCount = selectedTags.Count;

            // not selector
            foreach (var filter in filters)
            {

                fname = "-nsr-c";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string category = filter.Substring(fname.Length);
                    foreach (var tag in tags)
                    {
                        if (tag.Category == category)
                        {
                            if (selectedTags.Contains(tag))
                                selectedTags.Remove(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var tag in tags)
                    {
                        if (tag.Id != id)
                        {
                            if (selectedTags.Contains(tag))
                                selectedTags.Remove(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string name = filter.Substring(fname.Length);
                    foreach (var tag in tags)
                    {
                        if (tag.Name != name)
                        {
                            if (selectedTags.Contains(tag))
                                selectedTags.Remove(tag);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                if (!parseSuccessfull)
                    return tags;
            }

            if (selectedTags.Count == oldCount && filters.Any(x => x.Substring(0, 4) == "-nsr"))
            {
                parseSuccessfull = false;
                return tags;
            }

            return selectedTags;
        }
    }
}
