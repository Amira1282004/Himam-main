using Himam_main.Authorization;
using Himam_main.Models;
using Himam_main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Himam_main.Data;

namespace Himam_main.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.ManageServices)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class ServicesController : Controller
{
    private readonly IContentService _content;

    public ServicesController(IContentService content)
    {
        _content = content;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _content.GetServiceCategoriesAsync();
        return Json(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ServiceCategory input)
    {
        var item = await _content.SaveServiceCategoryAsync(input);
        return Json(new { success = true, item.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ServiceCategory input)
    {
        var item = await _content.SaveServiceCategoryAsync(input, id);
        return Json(new { success = true, item.Id });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _content.DeleteServiceCategoryAsync(id);
        return ok ? Json(new { success = true }) : NotFound();
    }
}

[Area("Admin")]
[Authorize(Policy = AppPolicies.ManageUsers)]
[Route("Admin/Api/[controller]")]
[IgnoreAntiforgeryToken]
public class UsersController : Controller
{
    private readonly HimanAlhayahContext _context;

    public UsersController(HimanAlhayahContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Include(u => u.Roles)
            .OrderBy(u => u.FullName)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Username,
                u.Email,
                Roles = u.Roles.Select(r => r.Name).ToList(),
                u.UpdatedAt
            })
            .ToListAsync();

        return Json(new { total = users.Count, items = users });
    }
}
