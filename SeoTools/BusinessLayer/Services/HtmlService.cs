using System.Net;
using System.Text;
using System.Web;
using BusinessLayer.Repositories;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace BusinessLayer.Services;

public class HtmlService : IHtmlService
{
    private readonly HttpClient _httpClient;
    private readonly IS3Service _s3Service;
    private const string GooglebotUserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 5X Build/MMB29P) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(x => x.StatusCode is HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMinutes(10),10));

    public HtmlService(IHttpClientFactory httpClientFactory, IS3Service s3Service)
    {
        _httpClient = httpClientFactory.CreateClient();
        _s3Service = s3Service;
    }


    public async Task<string> DownloadHtmlAsync(Uri url,string sitename)
    {
        
        // var request = new HttpRequestMessage(HttpMethod.Get, url);
        // string filename = "";
        // request.Headers.UserAgent.ParseAdd(GooglebotUserAgent);
        //
        // var response = await _httpClient.SendAsync(request);
        // if (response.IsSuccessStatusCode)
        // {
        //     var data = await response.Content.ReadAsStringAsync();
        //     filename =
        //         $@"{sitename}/{HttpUtility.UrlDecode(url.ToString().Replace("https://", "").Replace("/", "-"))}{new Random().Next()}.html";
        //     
        //     await SaveHtmlToS3Async(data,filename ,"name");
        // }
        //
        // return filename;
        
        
        
        
        try
        {
            while (true) // loop to allow for retries
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.UserAgent.ParseAdd(GooglebotUserAgent);

                    // Assuming _httpClient is an instance of HttpClient that's reused
                    var response = await _retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request));

                    var data = await response.Content.ReadAsStringAsync();

                    // Parse the HTML and extract the title
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(data);

                    var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
                    string title = titleNode != null ? titleNode.InnerText : "No title found";

                    if (title == "خطای ۴۲۹") // check if title is "خطای ۴۲۹"
                    {
                        await Task.Delay(60000); // wait for 60 seconds before retrying
                        continue; // retry the request
                    }

                    return title;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        

    }

    public async Task SaveHtmlToS3Async(string htmlContent, string filePathInS3,string fileName)
    {
        // Convert HTML content to a stream
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlContent));
        
        // Use the S3 service to upload the file
        await _s3Service.UploadFileAsync(stream, filePathInS3, fileName);
    }
}