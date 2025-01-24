using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.StorageFiles;
using SkyBox.Domain.Abstractions.Files;
using SkyBox.Domain.Models.File;

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
    
    [HttpPost]
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

    [HttpGet("{fileId:Guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFile([FromRoute] Guid fileId)
    {
        var fileStreamResult = await _fileStorageService.GetByIdAsync(fileId);
        
        if (fileStreamResult.IsFailed)
        {
            return BadRequest(fileStreamResult.Errors);
        }

        var result = File(
            fileStreamResult.Value.fileStream, 
            fileStreamResult.Value.storageFile.MimeType, 
            fileStreamResult.Value.storageFile.Name
        );
        
        return result;
    }
    
    // Берем из параметров запроса (FromQuery) когда нужна фильтрация,
    // сортировки или поиск. Когда ресурс, к которому осуществляется обращение,
    // остаётся тот же, но с определёнными условиями.
    // files?userId=value
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetStorageFileResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserFiles([FromQuery] Guid userId)
    {
        var userFiles = await _fileStorageService.GetByUserIdAsync(userId);
        
        if (userFiles.IsFailed)
        {
            return BadRequest(userFiles.Errors);
        }

        var result = _mapper.Map<IEnumerable<StorageFile>>(userFiles.Value);
        return Ok(result);
    }
    
    [HttpDelete("{fileId:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetStorageFileResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid fileId)
    {
        var deletedFile = await _fileStorageService.DeleteFileAsync(fileId);
        
        if (deletedFile.IsFailed)
        {
            return BadRequest(deletedFile.Errors);
        }

        var result = _mapper.Map<GetStorageFileResponse>(deletedFile.Value);
        return Ok(result);
    }
}
