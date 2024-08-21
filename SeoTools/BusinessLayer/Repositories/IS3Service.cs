namespace BusinessLayer.Repositories;

public interface IS3Service
{
    public Task UploadFileAsync(Stream fileStream, string path,string fileName);
}