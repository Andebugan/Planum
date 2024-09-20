# Config
[back](../Planum.md)

Tools for configuration loading, using and saving.
Replace global config for path with config, specific to exe file, from which all needed data is loaded

- [x] [ConfigLoader](./ConfigLoader.cs) - generic loader and saver for configurations, uses Newton.Json for seralization and deserialization of provided data. 

**Important note** - newton json can't work with complex types of collections, which must be either serialized/deserialized manualy or converted into simpler data type via DTO object
