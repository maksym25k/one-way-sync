# One way folder synchronizer in CSharp

This is a console app created using C#, which synchronizes two folders - source and target. 

Synchronization is one way only, meaning changes are syncrhonized only into the target folder, not the other way around.

The app logs changes and errors into the log file in the destination provided on startup and into the console.

After start, the app runs periodically, each new syncrhonization starts an x amount of ms after the previous one. 

The sync interval is provided by the user on startup.

The app uses lazy caching, so the amount of disk reads required is minimal.

The comparator assumes, that if file size and the time it was last written into are the same for both files, they are the same.

## Run args
To run this app you need to provide arguments in this order:
- Source folder (path)
- Target folder (path)
- Log path (If there is no file specified in the path, Log.txt will be created) 
- Interval (ms) interval between runs. The default value is 60000ms or 60s

```bash
./app ../source ../target ./log.txt 5000
```

##Updates tbd
- Caching and or comparing files using MD5 seems cool but its not really saving very much time and IO and adds complexity.
I will probably remove it from the Comparator and check only for timestamps and length difference.
