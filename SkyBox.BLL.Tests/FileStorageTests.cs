using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Moq;
using SkyBox.BLL.Services;
using SkyBox.Domain.Abstractions.Files;
using SkyBox.Domain.Models;

namespace SkyBox.BLL.Tests;

public class FileStorageTests
{
    private readonly FileStorageService _fileStorageService;
    private readonly Mock<IFileStorageRepository> _fileStorageRepositoryMock;
    private readonly Mock<IAmazonS3> _s3ClientMock;

    public FileStorageTests()
    {
        _fileStorageRepositoryMock = new Mock<IFileStorageRepository>();
        _s3ClientMock = new Mock<IAmazonS3>();
        Mock<ILogger<FileStorageService>> loggerMock = new();
        
        _fileStorageService = new FileStorageService(
            _s3ClientMock.Object,
            _fileStorageRepositoryMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_FailResult()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _fileStorageRepositoryMock
            .Setup(repo => repo.GetByIdAsync(fileId))
            .ReturnsAsync((StorageFile?)null);

        // Act
        var result = await _fileStorageService.GetByIdAsync(fileId);

        // Assert
        const string fileWithIdNotFoundPattern = "Файл с Id {0} не найден.";
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.Equal(string.Format(fileWithIdNotFoundPattern, fileId), result.Errors[0].Message);
    }
    
    [Fact]
    public async Task GetByIdAsync_Should_Return_FailResult_With_Exception()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        const string exceptionMessage = "S3 Exception"; 
        
        var storageFile = new StorageFile
        {
            Id = fileId,
            BucketName = "test",
            Name = "test",
            MimeType = "text/plain",
            Extension = ".txt",
            StoragePath = "test",
            UserId = Guid.NewGuid()
        };
        
        _fileStorageRepositoryMock
            .Setup(repo => repo.GetByIdAsync(fileId))
            .ReturnsAsync(storageFile);
         
        _s3ClientMock
            .Setup(client => client.GetObjectAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new Exception(exceptionMessage));
        
        // Act
        var result = await _fileStorageService.GetByIdAsync(fileId);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.Equal(exceptionMessage, result.Errors[0].Message);
    }
    
    [Fact]
    public async Task GetByIdAsync_Should_Return_SuccessResult()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        
        var storageFile = new StorageFile
        {
            Id = fileId,
            BucketName = "test",
            Name = "test",
            MimeType = "text/plain",
            Extension = ".txt",
            StoragePath = "test",
            UserId = Guid.NewGuid()
        };
        var responseStream = new MemoryStream([1, 2, 3]);
        
        _fileStorageRepositoryMock
            .Setup(repo => repo.GetByIdAsync(fileId))
            .ReturnsAsync(storageFile);

        _s3ClientMock
            .Setup(client => client.GetObjectAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetObjectResponse
            {
                ResponseStream = responseStream
            });
        
        // Act
        var result = await _fileStorageService.GetByIdAsync(fileId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.Equal(storageFile, result.Value.storageFile);
        Assert.NotNull(result.Value.fileStream);
    }
}