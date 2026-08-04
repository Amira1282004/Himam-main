using Himam_main.Authorization;
using Himam_main.Data;
using Himam_main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Himam_main;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.EditContent)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class CompanyInfoController : Controller
{
    private readonly HimanAlhayahContext _context;

    public CompanyInfoController(HimanAlhayahContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _context.CompanyInfos
            .Include(c => c.User)
            .ToListAsync();
        
        return Json(items.Select(c => new
        {
            c.Id,
            c.CompanyName,
            c.CompanyNameEn,
            c.Tagline,
            c.Email,
            c.Phone,
            c.Website,
            c.CreatedAt,
            c.UpdatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var companyInfo = await _context.CompanyInfos
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (companyInfo is null)
            return NotFound();
        
        return Json(companyInfo);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CompanyInfoInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var companyInfo = new CompanyInfo
        {
            CompanyName = input.CompanyName,
            CompanyNameEn = input.CompanyNameEn,
            Tagline = input.Tagline,
            TaglineEn = input.TaglineEn,
            Description = input.Description,
            DescriptionEn = input.DescriptionEn,
            Logo = input.Logo,
            Favicon = input.Favicon,
            Address = input.Address,
            Phone = input.Phone,
            Email = input.Email,
            Website = input.Website,
            CommercialRegister = input.CommercialRegister,
            TaxNumber = input.TaxNumber,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UserId = userId.Value
        };

        _context.CompanyInfos.Add(companyInfo);
        await _context.SaveChangesAsync();

        return Json(new { success = true, companyInfo.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CompanyInfoInput input)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var companyInfo = await _context.CompanyInfos.FindAsync(id);
        if (companyInfo is null)
            return NotFound();

        companyInfo.CompanyName = input.CompanyName;
        companyInfo.CompanyNameEn = input.CompanyNameEn;
        companyInfo.Tagline = input.Tagline;
        companyInfo.TaglineEn = input.TaglineEn;
        companyInfo.Description = input.Description;
        companyInfo.DescriptionEn = input.DescriptionEn;
        companyInfo.Logo = input.Logo;
        companyInfo.Favicon = input.Favicon;
        companyInfo.Address = input.Address;
        companyInfo.Phone = input.Phone;
        companyInfo.Email = input.Email;
        companyInfo.Website = input.Website;
        companyInfo.CommercialRegister = input.CommercialRegister;
        companyInfo.TaxNumber = input.TaxNumber;
        companyInfo.UpdatedAt = DateTime.Now;
        companyInfo.UserId = userId.Value;

        await _context.SaveChangesAsync();

        return Json(new { success = true, companyInfo.Id });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AppPolicies.ManagePages)]
    public async Task<IActionResult> Delete(int id)
    {
        var companyInfo = await _context.CompanyInfos.FindAsync(id);
        if (companyInfo is null)
            return NotFound();

        _context.CompanyInfos.Remove(companyInfo);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    public class CompanyInfoInput
    {
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyNameEn { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;
        public string TaglineEn { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Favicon { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string CommercialRegister { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
    }
}