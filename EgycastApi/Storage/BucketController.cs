using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace EgycastApi.Storage;

[ApiController]
[Route("buckets")]
public class BucketController : ControllerBase
{
    private readonly IAmazonS3 _s3Client;

    public BucketController(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    [HttpPost]
    public async Task<IResult> CreateBucket([FromQuery] string bucketName)
    {
        await _s3Client.PutBucketAsync(bucketName);
        return Results.Ok();
    }

    [HttpGet]
    public async Task<IResult> GetBuckets()
    {
        var data = await _s3Client.ListBucketsAsync();
        var buckets = data.Buckets.Select(bucket => bucket.BucketName);
        return Results.Ok(buckets);
    }
    
    [HttpDelete]
    public async Task<IResult> DeleteBucket([FromQuery] string bucketName)
    {
        await _s3Client.DeleteBucketAsync(bucketName);
        return Results.NoContent();
    }
}