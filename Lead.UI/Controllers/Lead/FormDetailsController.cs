using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Models.Lead;
using Infrustructure.Repositories.Data;
using Common.Utils.Helper;

namespace Lead.UI.Controllers.Lead;

public class FormDetailsController : Controller
{
    private readonly LeadContext _context;

    public FormDetailsController(LeadContext context)
    {
        _context = context;
    }

    // GET: FormDetails
    public async Task<IActionResult> Index()
    {
        var leadContext = _context.FormDetails.Include(f => f.BootstrapDataType).Include(f => f.CSharpDataType);
        return View(await leadContext.ToListAsync());
    }

    // GET: FormDetails/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var formDetails = await _context.FormDetails
            .Include(f => f.BootstrapDataType)
            .Include(f => f.CSharpDataType)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (formDetails == null)
        {
            return NotFound();
        }

        return View(formDetails);
    }

    // GET: FormDetails/Create
    public IActionResult Create()
    {
        ViewData["BootstrapTypeId"] = new SelectList(_context.DataTypes, "Id", "Id");
        ViewData["CSharpTypeId"] = new SelectList(_context.DataTypes, "Id", "Id");
        return View();
    }

    // POST: FormDetails/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,CSharpTypeId,BootstrapTypeId,IsNullSupported,IsSelectInput,IsActive,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] FormDetails formDetails)
    {
        if (ModelState.IsValid)
        {
            formDetails.CreatedBy = "sakib";
            formDetails.CreatedDate = TimeHelper.GetCurrentBangladeshTime();
            _context.Add(formDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["BootstrapTypeId"] = new SelectList(_context.DataTypes, "Id", "Id", formDetails.BootstrapTypeId);
        ViewData["CSharpTypeId"] = new SelectList(_context.DataTypes, "Id", "Id", formDetails.CSharpTypeId);
        return View(formDetails);
    }

    // GET: FormDetails/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var formDetails = await _context.FormDetails.FindAsync(id);
        if (formDetails == null)
        {
            return NotFound();
        }
        ViewData["BootstrapTypeId"] = new SelectList(_context.DataTypes, "Id", "Id", formDetails.BootstrapTypeId);
        ViewData["CSharpTypeId"] = new SelectList(_context.DataTypes, "Id", "Id", formDetails.CSharpTypeId);
        return View(formDetails);
    }

    // POST: FormDetails/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CSharpTypeId,BootstrapTypeId,IsNullSupported,IsSelectInput,IsActive,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] FormDetails formDetails)
    {
        if (id != formDetails.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                formDetails.ModifiedBy = "sakib";
                formDetails.ModifiedDate = TimeHelper.GetCurrentBangladeshTime();
                _context.Update(formDetails);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormDetailsExists(formDetails.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["BootstrapTypeId"] = new SelectList(_context.DataTypes, "Id", "Id", formDetails.BootstrapTypeId);
        ViewData["CSharpTypeId"] = new SelectList(_context.DataTypes, "Id", "Id", formDetails.CSharpTypeId);
        return View(formDetails);
    }

    // GET: FormDetails/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var formDetails = await _context.FormDetails
            .Include(f => f.BootstrapDataType)
            .Include(f => f.CSharpDataType)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (formDetails == null)
        {
            return NotFound();
        }

        return View(formDetails);
    }

    // POST: FormDetails/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var formDetails = await _context.FormDetails.FindAsync(id);
        if (formDetails != null)
        {
            _context.FormDetails.Remove(formDetails);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FormDetailsExists(int id)
    {
        return _context.FormDetails.Any(e => e.Id == id);
    }
}
