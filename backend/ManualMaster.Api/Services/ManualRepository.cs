using ManualMaster.Api.Data;
using ManualMaster.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ManualMaster.Api.Services;

public class ManualRepository : IManualRepository
{
    private readonly ManualDbContext _context;

    public ManualRepository(ManualDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Manual>> GetManualsAsync(string? category, string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Manuals.AsNoTracking().OrderByDescending(m => m.UploadDate).AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(m => m.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(m => EF.Functions.ILike(m.Title, pattern) || EF.Functions.ILike(m.Content, pattern));
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountManualsAsync(string? category, string? search, CancellationToken cancellationToken)
    {
        var query = _context.Manuals.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(m => m.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(m => EF.Functions.ILike(m.Title, pattern) || EF.Functions.ILike(m.Content, pattern));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Manuals
            .Select(m => m.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public Task<Manual?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _context.Manuals.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task AddAsync(Manual manual, CancellationToken cancellationToken)
    {
        await _context.Manuals.AddAsync(manual, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Manual manual, CancellationToken cancellationToken)
    {
        _context.Manuals.Update(manual);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Manual manual, CancellationToken cancellationToken)
    {
        _context.Manuals.Remove(manual);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
