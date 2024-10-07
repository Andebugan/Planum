# Task file management
[back](../PlanumRepo.md)

Main components:
- [ITaskFileManager.cs](./ITaskFileManager.cs) - interface for accessing task files
- [TaskFileManager.cs](./TaskFileManager.cs) - ITaskFileManager implementation
- [TaskMarkdownReader.cs](./TaskMarkdownReader.cs) - read formatted task data from file, **generates missing id's on read**
- [TaskMarkdownWriter.cs](./TaskMarkdownWriter.cs) - write/update task data as formatted plain text
