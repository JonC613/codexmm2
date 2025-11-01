using ManualMaster.Api.Dtos;
using ManualMaster.Api.Services;
using ManualMaster.Api.Services.AutoFind;
using Microsoft.AspNetCore.Mvc;

namespace ManualMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManualsController : ControllerBase
{
    private readonly IManualService _manualService;
    private readonly IAutoFindService _autoFindService;
    private readonly ILogger<ManualsController> _logger;

    public ManualsController(IManualService manualService, IAutoFindService autoFindService, ILogger<ManualsController> logger)
    {
        _manualService = manualService;
        _autoFindService = autoFindService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetManuals([FromQuery] ManualQueryParameters query, CancellationToken cancellationToken)
    {
        var (items, total) = await _manualService.GetManualsAsync(query, cancellationToken);
        return Ok(new
        {
            items,
            total,
            page = Math.Max(1, query.Page),
            pageSize = Math.Clamp(query.PageSize, 1, 100)
        });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _manualService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetManual(int id, CancellationToken cancellationToken)
    {
        var manual = await _manualService.GetManualAsync(id, cancellationToken);
        return manual is null ? NotFound() : Ok(manual);
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> DownloadManual(int id, CancellationToken cancellationToken)
    {
        var file = await _manualService.GetManualFileAsync(id, cancellationToken);
        if (file is null)
        {
            return NotFound();
        }

        return File(file.Value.Data, file.Value.ContentType, file.Value.FileName);
    }

    [HttpPost]
    public async Task<IActionResult> CreateManual([FromBody] ManualCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var manual = await _manualService.CreateManualAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetManual), new { id = manual.Id }, manual);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateManual(int id, [FromBody] ManualUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var manual = await _manualService.UpdateManualAsync(id, request, cancellationToken);
        return manual is null ? NotFound() : Ok(manual);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteManual(int id, CancellationToken cancellationToken)
    {
        var deleted = await _manualService.DeleteManualAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("auto-find")]
    public async Task<IActionResult> AutoFind([FromBody] AutoFindRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        _logger.LogInformation("Auto-find requested for {Product} {Model}", request.ProductName, request.ModelNumber);
        var results = await _autoFindService.SearchAsync(request, cancellationToken);
        return Ok(results);
    }
}
