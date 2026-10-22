
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class NotesController : Controller
{
    private readonly NoteRepository _repository;

    public NotesController(NoteRepository repository)
    {
        _repository = repository;
    }

    // Display all notes
    public async Task<IActionResult> Index()
    {
        var notes = await _repository.GetAllAsync();
        return View(notes);
    }

    // Show the Add Note page
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Note());
    }

    // Save a new note
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Note note)
    {
        if (!ModelState.IsValid)
            return View(note);

        await _repository.AddAsync(note);
        return RedirectToAction(nameof(Index));
    }

    // Show the Edit Note page
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var note = await _repository.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        return View(note);
    }

    // Save changes to a note
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Note note)
    {
        if (!ModelState.IsValid)
            return View(note);

        var existingNote =
            await _repository.GetByIdAsync(note.NoteId);

        if (existingNote == null)
            return NotFound();

        await _repository.UpdateAsync(note);
        return RedirectToAction(nameof(Index));
    }

    // Show the Delete confirmation page
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var note = await _repository.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        return View(note);
    }

    // Delete the selected note
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var note = await _repository.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
