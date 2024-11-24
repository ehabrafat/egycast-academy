using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using EgycastApi.Storage.Dtos;
using Microsoft.Extensions.Options;

namespace EgycastApi.Storage;

public class FileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly AWSOptions _awsOptions;
    
    public FileService(IAmazonS3 s3Client, IOptions<AWSOptions> awsOptions)
    {
        _s3Client = s3Client;
        _awsOptions = awsOptions.Value;
    }

    
    public async Task<List<FileResDto>> GetAllFiles(string bucketName, string? prefix)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(bucketName);
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix
            };
            var result = await _s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(obj =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = obj.Key,
                    Expires = DateTime.UtcNow.AddDays(1)
                };
                return new FileResDto { Name = obj.Key, Url = _s3Client.GetPreSignedURL(urlRequest) };
            }).ToList();
            return s3Objects;
            
        }
        catch
        {
            return [];
        }
    }

    
    public async Task UploadFiles(List<IFormFile> files, string bucketName, string? prefix, CancellationToken cancellationToken)
    {
        await _s3Client.EnsureBucketExistsAsync(bucketName);
        var tasks = files.Select(file => UploadFile(file, bucketName, prefix, cancellationToken));
        await Task.WhenAll(tasks);
    }
    
    public async Task UploadFile(IFormFile file, string bucketName, string? prefix, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var req = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = String.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix.Trim('/')}/{file.FileName}",
            InputStream = file.OpenReadStream()
        };
        req.Metadata.Add("Content-Type", file.ContentType);
        await _s3Client.PutObjectAsync(req);
    }
}