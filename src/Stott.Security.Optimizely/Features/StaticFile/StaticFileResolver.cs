namespace Stott.Security.Optimizely.Features.StaticFile;

using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.Extensions.FileProviders;

internal sealed class StaticFileResolver : IStaticFileResolver
{
    public byte[] GetFileContent(string staticFileName)
    {
        var assembly = Assembly.GetAssembly(typeof(StaticFileResolver));
        var fileProvider = new EmbeddedFileProvider(assembly!);
        var directoryContents = fileProvider.GetDirectoryContents(string.Empty);

        foreach (var fileInfo in directoryContents)
        {
            if (fileInfo.Name.EndsWith(staticFileName, StringComparison.OrdinalIgnoreCase))
            {
                using var memoryStream = new MemoryStream();
                using var stream = fileInfo.CreateReadStream();

                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }

        return Array.Empty<byte>();
    }

    public string GetJavaScriptPath()
    {
        return GetFileName(@"(main\.)[a-z0-9]{1,10}(.js)$");
    }

    public string GetStyleSheetPath()
    {
        return GetFileName(@"(main\.)[a-z0-9]{1,10}(.css)$");
    }

    public string GetFileMimeType(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        if (fileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
        {
            return "application/javascript";
        }

        if (fileName.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
        {
            return "text/css";
        }

        if (fileName.EndsWith(".map", StringComparison.OrdinalIgnoreCase))
        {
            return "application/json";
        }

        return "text/plain";
    }

    private static string GetFileName(string regexPattern)
    {
        var assembly = Assembly.GetAssembly(typeof(StaticFileResolver));
        var fileProvider = new EmbeddedFileProvider(assembly!);
        var directoryContents = fileProvider.GetDirectoryContents(string.Empty);
        var regEx = new Regex(regexPattern, RegexOptions.IgnoreCase);

        foreach (var fileInfo in directoryContents)
        {
            if (regEx.IsMatch(fileInfo.Name))
            {
                return regEx.Match(fileInfo.Name).Value;
            }
        }

        return string.Empty;
    }
}