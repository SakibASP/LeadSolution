using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Models.Lead;
using Infrustructure.Repositories.Data;

namespace Lead.UI.Controllers.Lead;

public class DataTypesController : Controller
{
    private readonly LeadContext _context;

    public DataTypesController(LeadContext context)
    {
        _context = context;
    }

    // GET: DataTypes
    public async Task<IActionResult> Index()
    {
        return View(await _context.DataTypes.ToListAsync());
    }

    // GET: DataTypes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var dataTypes = await _context.DataTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (dataTypes == null)
        {
            return NotFound();
        }

        return View(dataTypes);
    }

    // GET: DataTypes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: DataTypes/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,IsBootstrap")] DataTypes dataTypes)
    {
        if (ModelState.IsValid)
        {
            _context.Add(dataTypes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(dataTypes);
    }

    // GET: DataTypes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var dataTypes = await _context.DataTypes.FindAsync(id);
        if (dataTypes == null)
        {
            return NotFound();
        }
        return View(dataTypes);
    }

    // POST: DataTypes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsBootstrap")] DataTypes dataTypes)
    {
        if (id != dataTypes.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(dataTypes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataTypesExists(dataTypes.Id))
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
        return View(dataTypes);
    }

    // GET: DataTypes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var dataTypes = await _context.DataTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (dataTypes == null)
        {
            return NotFound();
        }

        return View(dataTypes);
    }

    // POST: DataTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var dataTypes = await _context.DataTypes.FindAsync(id);
        if (dataTypes != null)
        {
            _context.DataTypes.Remove(dataTypes);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DataTypesExists(int id)
    {
        return _context.DataTypes.Any(e => e.Id == id);
    }
}
