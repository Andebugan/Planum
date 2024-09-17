# Planum
[back](../Planum.md)

- [ ] Divide solution into several projects:
    - [ ] PlanumConsole
    - [ ] PlanumModel
    - [ ] PlanumRepo

- [x] [Config](./Config/Config.md) - saving and loading configuration
    - [x] Replace file watch list with simple dotfile with paths to local watchfiles, which will in turn contain paths to wathed task files
- [ ] [Console](./Console/Console.md) - console interface and commands for it
- [x] [Logger](./Logger/Logger.md) - logger wrapper, logger functions
- [x] [Model](./Model/Model.md) - entities and buisness logic
- [x] [Parser](./Parser/Parser.md) - specialized string parsers for common values and specific task values
- [x] [Repo](./Repo/Repo.md) - concrete realization of repository
    - [x] Replace links to files with recursive markdown directory parsing
