using Npgsql;
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _members;

    public MembersController(MemberRepository members)
    {
        _members = members;
    }

    // READ: display all members.
    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();
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
            // 2. Show the form again, with what was typed.
            return View(member);
        }

        try
        {
            // 3. Save the new member.
            await _members.AddAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // 5. Put a message under the Email box.
            ModelState.AddModelError("Email", "Another member already uses this email.");
            // 6. Show the form again, with what was typed.
            return View(member);
        }

        // 7. The member was saved: go back to the list.
        return RedirectToAction(nameof(Index));
    }

    // EDIT: display the existing member.
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var member = await _members.GetByIdAsync(id);

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
            // 2. Show the form again, with what was typed.
            return View(member);
        }

        try
        {
            // 3. Save the changes.
            await _members.UpdateAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // 5. Put a message under the Email box.
            ModelState.AddModelError("Email", "Another member already uses this email.");
            // 6. Show the form again, with what was typed.
            return View(member);
        }

        // 7. The changes were saved: go back to the list.
        return RedirectToAction(nameof(Index));
    }

    // DELETE: display the confirmation page.
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _members.GetByIdAsync(id);

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
        try
        {
            // 1. Delete the member.
            await _members.DeleteAsync(id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            // 3. Load the member again, to show them on the page.
            var member = await _members.GetByIdAsync(id);

            if (member == null)
            {
                // 5. Answer "not found".
                return NotFound();
            }

            // 6. Set the message the page will show.
            ViewBag.Error = "This member has loans on record, so the database will not delete them.";
            // 7. Show the Delete page again, with the member.
            return View("Delete", member);
        }

        // 8. The member was deleted: go back to the list.
        return RedirectToAction(nameof(Index));
    }
}