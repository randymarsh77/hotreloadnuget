using System;
using System.IO;
using System.Linq;

var path = Path.GetFullPath(args.FirstOrDefault() ?? ".");
if (!Directory.Exists(path))
{
	Console.Error.WriteLine($"Path does not exist: {path}");
	Environment.Exit(1);
}

using var watcher = new FileSystemWatcher();

watcher.Path = path;
watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
watcher.Filter = "*.nupkg";

watcher.Changed += OnChanged;
watcher.Created += OnChanged;
watcher.Renamed += OnChanged;

watcher.EnableRaisingEvents = true;

Console.WriteLine($"Watching for package changes in {path}");
Console.WriteLine("Press 'q' to stop hotreloadnuget.");
while (Console.Read() != 'q');

void OnChanged(object source, FileSystemEventArgs e) => ClearPackageCache(e.Name);

void ClearPackageCache(string package)
{
	var globalCacheLocation = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
	var name = string.Join(".", package.Split(".").Reverse().Skip(4).Reverse()).ToLowerInvariant();
	var path = Path.Join(globalCacheLocation, name);
	if (Directory.Exists(path))
	{
		Console.WriteLine($"Removing {path}");
		Directory.Delete(path, true);
	}
}
