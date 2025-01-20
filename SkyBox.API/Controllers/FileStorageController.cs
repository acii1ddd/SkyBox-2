using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.StorageFiles;
using SkyBox.Domain.Abstractions.Files;

namespace SkyBox.API.Controllers;

[ApiController]
[Route("api/files")]
public class FileStorageController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public FileStorageController(IFileStorageService fileStorageService, ILogger<FileStorageController> logger, IMapper mapper)
    {
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    /// <summary>
    /// Загрузить файл 
    /// </summary>
    /// <param name="file"></param>
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetStorageFileResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        // доступна в текущем блоке кода (контроллера)
        await using var stream = file.OpenReadStream();
        
        var uploadedFile = await _fileStorageService.UploadFileAsync(stream, file.FileName, file.ContentType);
        if (uploadedFile.IsFailed)
        {
            return BadRequest(uploadedFile.Errors);
        }

        var result = _mapper.Map<GetStorageFileResponse>(uploadedFile.Value);
        return Ok(result);
    } // stream.DisposeAsync();

    [HttpGet("download/{fileId:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFile([FromRoute] Guid fileId)
    {
        var result = await _fileStorageService.GetByIdAsync(fileId);
        
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        var fileStreamResult = File(
            result.Value.fileStream, 
            result.Value.storageFile.MimeType, 
            result.Value.storageFile.Name
        );
        return fileStreamResult;
    }
}
