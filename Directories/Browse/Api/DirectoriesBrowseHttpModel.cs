using Browser.Directories.Browse.Service;
using System;
using System.Collections.Immutable;
using System.Security.Cryptography.Xml;

namespace Browser.Directories.Browse.Api
{
    public class DirectoriesBrowseHttpModel
    {
        public DirectoriesBrowseHttpModel(DirectoryBrowserServiceResult serviceResult) 
        { 
            CurrentPath = serviceResult.CurrentPath;
            SearchTerm = serviceResult.SearchPattern;
            IncludeSubdirectories = serviceResult.IncludeSubdirectories;
            MatchedFileDirectories = GetMatchedFileDirectories(serviceResult);
            MatchedDirectories = GetMatchedDirectories(serviceResult);
        }

        public string? CurrentPath { get; set; }
        public string? SearchTerm { get; set; }
        public bool IncludeSubdirectories { get; set; }

        private readonly List<MatchedFileDirectory> _matchedFileDirectories = [];
        public IEnumerable<MatchedFileDirectory> MatchedFileDirectories 
        { 
            get => [.. _matchedFileDirectories]; 
            set
            {
                _matchedFileDirectories.Clear();

                if (value is not null)
                    _matchedFileDirectories.AddRange(value);
            } 
        }

        private readonly List<MatchedDirectory> _matchedDirectories = [];
        public IEnumerable<MatchedDirectory> MatchedDirectories
        { 
            get => [.._matchedDirectories]; 
            set
            {
                _matchedDirectories.Clear();

                if (value is not null)
                    _matchedDirectories.AddRange(value);
            }
        }

        private static List<MatchedDirectory> GetMatchedDirectories(DirectoryBrowserServiceResult serviceResult)
        {
            if (serviceResult?.MatchedDirectories is null || !serviceResult.MatchedDirectories.Any())
                return [];

            return [.. serviceResult.MatchedDirectories.Select(md => new MatchedDirectory()
            {
                Name = md.Name,
                Size = GetFileDisplaySize(md.SizeInBytes),
                FileCount = md.FileCount
            })];
        }

        private static List<MatchedFileDirectory> GetMatchedFileDirectories(DirectoryBrowserServiceResult serviceResult)
        {
            if (serviceResult?.MatchedFiles is null || !serviceResult.MatchedFiles.Any())
                return [];

            var matchedDirectoryNames = serviceResult.MatchedFiles
                .Select(mf => Path.GetDirectoryName(mf.PathName) ?? String.Empty)
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct()
                .ToArray();

            var allDirectoryNames = new List<string>(matchedDirectoryNames);

            if (!allDirectoryNames.Contains(Path.DirectorySeparatorChar.ToString()))
                allDirectoryNames.Add(Path.DirectorySeparatorChar.ToString());

            if (String.IsNullOrWhiteSpace(serviceResult.SearchPattern) ||
                serviceResult.SearchPattern.Equals("*") ||
                serviceResult.SearchPattern.Equals("*.*"))
            {
                // Check for missing segments that may not have had any matching
                // files but are part of the path to other matched files. This is
                // only relevant when searching with a specific pattern.
                foreach (var directoryName in matchedDirectoryNames)
                {
                    var pathSegments = directoryName.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var pathSegment in pathSegments)
                    {
                        var testPath = $"{Path.DirectorySeparatorChar}{Path.Combine([.. pathSegments.TakeWhile(s => s != pathSegment).Append(pathSegment)])}";

                        if (!allDirectoryNames.Contains(testPath))
                            allDirectoryNames.Add(testPath);
                    }
                }
            }

            allDirectoryNames.Sort();

            var directories = new List<MatchedFileDirectory>();

            foreach (var directoryName in allDirectoryNames)
            {
                var directory = new MatchedFileDirectory()
                {
                    Name = directoryName,
                    FileCount = serviceResult.MatchedFiles.Count(mf => mf.PathName!.StartsWith(directoryName)),
                    Size = GetFileDisplaySize(serviceResult.MatchedFiles
                                                           .Where(mf => mf.PathName!.StartsWith(directoryName))
                                                           .Sum(mf => mf.SizeInBytes)),
                    Files = [.. serviceResult.MatchedFiles.Where(mf => 
                                    {
                                        var dirName = Path.GetDirectoryName(mf.PathName);
                                        return dirName != null && dirName.Equals(directoryName);
                                    })
                                .Select(f => new MatchedFile(Path.GetFileName(f.PathName), GetFileDisplaySize(f.SizeInBytes)))]
                };

                directories.Add(directory);
            }

            return [.. directories];
        }

        private static string GetFileDisplaySize(long? fileSize)
        {
            return fileSize switch
            {
                null => "Invalid size",
                < 0 => "Invalid size",
                0 => "0 B",
                < 1024 => $"{fileSize} B",
                < 1048576 => $"{fileSize / 1024.0:F2} KB",
                < 1073741824 => $"{fileSize / 1048576.0:F2} MB",
                _ => $"{fileSize / 1073741824.0:F2} GB"
            };
    }
    }
}
