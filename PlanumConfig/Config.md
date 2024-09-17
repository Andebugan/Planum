# Config
[back](../Planum.md)

Tools for configuration loading, using and saving.

- [x] [ConfigLoader](./ConfigLoader.cs) - generic loader and saver for configurations, uses Newton.Json for seralization and deserialization of provided data. 
 [x] [AppConfig](./AppConfig.cs) - "root" configuration, contains **hardcoded path to app config**, which in turn contains paths to all other configurations plus some appwide settings
- [x] [CommandConfig](./CommandConfig.cs) - contains configuration for aliases and general command related settings
- [x] [RepoConfig](./RepoConfig.cs) - contains configuration for repository (markdown task format parsing settings) and dictionary of paths to watched files

**Important note** - newton json can't work with complex types of collections, which must be either serialized/deserialized manualy or converted into simpler data type via DTO object
