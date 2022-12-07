namespace Stott.Security.Optimizely.Features.StaticFile;

public interface IStaticFileResolver
{
    string GetJavaScriptPath();

    string GetStyleSheetPath();

    byte[] GetFileContent(string staticFileName);

    string GetFileMimeType(string fileName);
}