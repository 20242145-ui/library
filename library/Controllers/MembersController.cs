
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _repository;

    public MembersController(MemberRepository repository)
    {
        _repository = repository;
    }

    // READ: display all members.
    public async Task<IActionResult> Index()
    {
        var members = await _repository.GetAllAsync();
        return View(members);
    }

    // CREATE: display the form.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Member());
    }

    // CREATE: save the new member.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        await _repository.AddAsync(member);
        return RedirectToAction(nameof(Index));
    }

    // EDIT: display the existing member.
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var member = await _repository.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // EDIT: save the changes.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Member member)
    {
        if (id != member.MemberId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(member);
        }

        var existingMember = await _repository.GetByIdAsync(id);

        if (existingMember == null)
        {
            return NotFound();
        }

        await _repository.UpdateAsync(member);
        return RedirectToAction(nameof(Index));
    }

    // DELETE: display the confirmation page.
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _repository.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // DELETE: remove the member.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var member = await _repository.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
