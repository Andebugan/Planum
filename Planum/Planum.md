# Planum
[back](../Planum.md)

## Project structure
- [ ] [Config](./Config/Config.md) - saving and loading configuration
- [ ] [Console](./Console) - console interface and commands for it
- [ ] [Logger](./Logger/Logger.md) - logger wrapper, logger functions
- [ ] [Model](./Model) - entities and buisness logic
- [ ] [Parser](./Parser/Parser.md) - specialized string parsers for common values and specific task values
- [ ] [Repo](./Repo/Repo.md) - concrete realization of repository

## Global workflow

```
User - Shell - Command - Task     - Repo - writer
input          manager   managers        \ reader
                                              
```
