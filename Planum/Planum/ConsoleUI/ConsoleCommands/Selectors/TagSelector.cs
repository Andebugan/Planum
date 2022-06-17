using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TagSelector
    {
        public List<Tag> Select(List<string> filters, List<Tag> tags, ref bool parseSuccessfull,
            ITaskManager _taskManager, ITagManager _tagManager)
        {
            List<Tag> selectedTags = new List<Tag>();
            // selector
            foreach (var filter in filters)
            {
                if (filter.Length > 5 && filter.Substring(0, 5) == "-sr-c")
                {
                    bool added = false;
                    string category = filter.Substring(5);
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-sr-i")
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(5));
                    foreach (var tag in tags)
                    {
                        if (tag.Id == id)
                        {
                            selectedTags.Add(tag);
                            added = true;
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-sr-n")
                {
                    bool added = false;
                    string name = filter.Substring(5);
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
                        parseSuccessfull = false;
                }
                else if (filter.Length < 3 || (filter.Substring(0, 2) != "-f" && filter.Substring(0, 3) != "-sr"))
                {
                    parseSuccessfull = false;
                    break;
                }

                if (!parseSuccessfull)
                    break;
            }
            return selectedTags;
        }
    }
}
