using Amazon.S3;
using Amazon.S3.Model;
using BusinessLayer.Repositories;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Services;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration.GetValue<string>("AWS:BucketName");
    }

    public async Task UploadFileAsync(Stream fileStream, string path,string fileName)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = path,
            InputStream = fileStream,
            ContentType = "text/html" // Change accordingly
        };

        await _s3Client.PutObjectAsync(putRequest);
    }
}