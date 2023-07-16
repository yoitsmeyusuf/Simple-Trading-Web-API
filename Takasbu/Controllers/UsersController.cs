using Takasbu.Models;
using Takasbu.Services;
using Microsoft.AspNetCore.Mvc;

namespace Takasbu.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _UsersService;

    public UsersController(UsersService UsersService) =>
        _UsersService = UsersService;

    [HttpGet]
    public async Task<List<User>> Get() =>
        await _UsersService.GetAsync();

    [HttpGet]
    public async Task<ActionResult<User>> Get(string id)
    {
        var User = await _UsersService.GetAsync(id);

        if (User is null)
        {
            return NotFound();
        }

        return User;
    }

    

    [HttpPost]
public async Task<IActionResult> Post(User newUser, IFormFile file)
{
    if (file != null && file.Length > 0)
    {
        // Generate a unique file name or use the original file name
         string originalFileName = file.FileName; // Assuming you have the original file name
                string extension = Path.GetExtension(originalFileName);
                string fileName = Guid.NewGuid().ToString() + extension;


        // Set the file path where you want to save the uploaded files
        string filePath = Path.Combine(@"C:\pictures", fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        newUser.Picture = fileName;
    }

    await _UsersService.CreateAsync(newUser);

    return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
}

    [HttpPut]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var User = await _UsersService.GetAsync(id);

        if (User is null)
        {
            return NotFound();
        }

        updatedUser.Id = User.Id;

        await _UsersService.UpdateAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var User = await _UsersService.GetAsync(id);

        if (User is null)
        {
            return NotFound();
        }

        await _UsersService.RemoveAsync(id);

        return NoContent();
    }
}