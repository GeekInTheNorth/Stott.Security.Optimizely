namespace Stott.Security.Optimizely.Features.StaticFile;

public interface IStaticFileResolver
{
    string GetJavaScriptFileName();

    string GetStyleSheetFileName();

    byte[] GetFileContent(string staticFileName);

    string GetFileMimeType(string fileName);
}