using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yeni.Configuration;
using yeni.Domain.DTO.Requests;
using yeni.Domain.DTO.Responses;
using yeni.Domain.Repository;

namespace yeni.Controllers;

[ApiController]
[Route("api/memories")]
[Authorize]
public class MemoryController : ControllerBase
{
    private readonly MemoryService _memoryService;

    public MemoryController(MemoryService memoryService)
    {
        _memoryService = memoryService;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateMemory([FromForm] CreateMemoryRequest request, CancellationToken ct)
    {
        try
        {
            var userId = GetUserId();
            var result = await _memoryService.CreateMemoryWithAttachmentsAsync(request, userId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{memoryId}")]
    [Authorize]
    public async Task<IActionResult> GetMemory(int memoryId, CancellationToken ct)
    {
        var result = await _memoryService.GetMemoryByIdAsync(memoryId, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new {});
    }

    [HttpGet("my-memories")]
    public async Task<IActionResult> GetMyMemories(CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await _memoryService.GetUserMemoriesAsync(userId, ct);
        return Ok(result);
    }

    [HttpPost("{memoryId}/attachments")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddAttachments(int memoryId, [FromForm] List<IFormFile> files, [FromForm] List<string?>? captions, CancellationToken ct)
    {
        try
        {
            var userId = GetUserId();
            var result = await _memoryService.AddAttachmentsToMemoryAsync(memoryId, files, userId, captions, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{memoryId}/attachments/{attachmentId}")]
    public async Task<IActionResult> RemoveAttachment(int memoryId, int attachmentId, CancellationToken ct)
    {
        try
        {
            var userId = GetUserId();
            var result = await _memoryService.RemoveAttachmentFromMemoryAsync(memoryId, attachmentId, userId, ct);
            return Ok(new { success = result });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{memoryId}")]
    public async Task<IActionResult> DeleteMemory(int memoryId, CancellationToken ct)
    {
        try
        {
            var userId = GetUserId();
            var result = await _memoryService.DeleteMemoryAsync(memoryId, userId, ct);
            return Ok(new { success = result });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }



}
