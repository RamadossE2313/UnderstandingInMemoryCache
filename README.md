# How to add "InMemoryCache"

1. Install the package "Microsoft.Extensions.Caching.Memory"
2. Add "builder.Services.AddMemoryCache();" in service collections

## InMemory concept Used
1. How to set inmemory cache.
2. How to read value from inmemory cache.
3. How to clear the cache automatically on specfic time/ duration using AbsoluteExpiration property.
4. How to clear the cache automatically if cache not used in period of time using SlidingExpiration property.
5. How to call a method after cache get cleared using PostEvictionCallbacks property.
