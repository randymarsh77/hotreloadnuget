using System;
using System.IO;
using System.Linq;

using var watcher = new FileSystemWatcher();

watcher.Path = ".";

watcher.NotifyFilter = NotifyFilters.LastAccess
	| NotifyFilters.LastWrite
	| NotifyFilters.FileName
	| NotifyFilters.DirectoryName;

watcher.Filter = "*.nupkg";

watcher.Changed += OnChanged;
watcher.Created += OnChanged;
watcher.Deleted += OnChanged;
watcher.Renamed += OnChanged;

watcher.EnableRaisingEvents = true;

Console.WriteLine("Press 'q' to stop hotreloadnuget.");
while (Console.Read() != 'q');

void OnChanged(object source, FileSystemEventArgs e) => ClearPackageCache();

void ClearPackageCache()
{
	var globalCacheLocation = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
	var packages = Directory.GetFiles(".").Where(x => x.EndsWith(".nupkg", StringComparison.InvariantCulture));
	foreach (var package in packages)
	{
		var name = string.Join(".", package.Split(".").Skip(1).Reverse().Skip(4).Reverse()).ToLowerInvariant().Replace($"{Path.DirectorySeparatorChar}", "", StringComparison.InvariantCulture);
		var path = Path.Join(globalCacheLocation, name);
		if (Directory.Exists(path))
		{
			Console.WriteLine($"Removing {path}");
			Directory.Delete(path, true);
		}
	}
}
