using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Infrustructure.Repositories.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lead.UI.Controllers.Lead;

public class FormValuesController : Controller
{
    private readonly LeadContext _context;

    public FormValuesController(LeadContext context)
    {
        _context = context;
    }

    // GET: /FormValues/DynamicForm
    public async Task<IActionResult> DynamicForm()
    {
        var formDetails = await _context.FormDetails
            .Include(f => f.BootstrapDataType)
            .Where(f => f.IsActive)
            .ToListAsync();

        var vm = new DynamicFormViewModel
        {
            Inputs = [.. formDetails.Select(f => new DynamicFormDto
            {
                FormDetailId = f.Id,
                Label = f.Name,
                InputType = f.BootstrapDataType?.Name ?? "text",
                IsSelectInput = f.IsSelectInput
            })]
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> DynamicForm(DynamicFormViewModel model)
    {
        var submissionId = await _context.FormValues.MaxAsync(x => x.SubmissionId) ?? 1;
        foreach (var input in model.Inputs)
        {
            var entity = new FormValues
            {
                FormId = input.FormDetailId,
                FormValue = input.Value,
                CreatedDate = DateTime.Now,
                SubmissionId = (submissionId + 1)
            };

            _context.FormValues.Add(entity);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // List view
    public async Task<IActionResult> Index()
    {
        // 1. Get all FormDetail names
        ViewBag.Headers = await _context.FormDetails
            .AsNoTracking()
            .OrderBy(f => f.Id)
            .Select(f => f.Name)
            .ToListAsync();
        var values = await _context.FormValues
            .AsNoTracking()
            .ToListAsync();

        return View(values);
    }
}
