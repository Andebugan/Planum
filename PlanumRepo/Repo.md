# Repository
[back](../Planum.md)

Concrete implementation for ITaskRepo interface

Main components:
- [x] [ITaskFileManager.cs](./ITaskFileManager.cs) - interface for accessing task files
- [x] [TaskFileManager.cs](./TaskFileManager.cs) - ITaskFileManager implementation
- [x] [TaskReader.cs](./TaskReader.cs) - read formatted task data from file
- [x] [TaskWriter.cs](./TaskWriter.cs) - write/update task data as formatted plain text

How repo *should* work:
1. [x] Config contains directories, that can be parsed recursively for markdown files that could contain markdown tasks (probably must be async and run in parallel because parsing can take a lot of time)
2. [x] At the start or via load command Repo should go through markdown files and collect tasks from them, each task should have it's own file field, that specifies, from which file it came
3. [x] At save tasks are "returned" back into their respective files, or removed from them in case of deletion

Task can have **multiple files** in which case it must be synchronized on each edit or deletion
