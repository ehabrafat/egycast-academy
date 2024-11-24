using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace EgycastApi.Storage;

[ApiController]
[Route("files")]
public class FileController : ControllerBase
{
    private readonly IAmazonS3 _s3Client;
    private readonly FileService _fileService;
    public FileController(IAmazonS3 s3Client, FileService fileService)
    {
       _s3Client = s3Client;
       _fileService = fileService;
    }

    [HttpPost("{bucketName}")]
    public async Task<IResult> UploadFile(IFormFile file, string bucketName, string? prefix, CancellationToken cancellationToken)
    {
        await _fileService.UploadFile(file, bucketName, prefix, cancellationToken);
        return Results.Ok($"File {prefix}/{file.FileName} uploaded successfully!");
    }

    [HttpGet("{bucketName}")]
    public async Task<IResult> GetAllFiles(string bucketName, string? prefix)
    {
        var files = await _fileService.GetAllFiles(bucketName, prefix);
        return Results.Ok(files);
    }
    
    [HttpGet("{bucketName}/{key}")]
    public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
    {
        var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
        return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
    }
}