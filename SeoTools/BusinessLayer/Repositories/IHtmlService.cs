namespace BusinessLayer.Repositories;

public interface IHtmlService
{
    public Task<string> DownloadHtmlAsync(Uri url,string sitename);
    public Task SaveHtmlToS3Async(string htmlContent, string filePathInS3,string fileName);

}