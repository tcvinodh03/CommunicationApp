using CommunicationAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy ="RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users.OrderBy(u => u.UserName).Select(s => new
            {
                s.Id,
                s.UserName,
                Roles = s.UserRoles.Select(u => u.Role.Name).ToList(),
            }).ToListAsync();
            return Ok(users);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admin or Moderator");
        }

        [Authorize(Policy ="RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string userName,[FromQuery]string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("No role selected");
            var selectedRoles = roles.Split(',');
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add the roles");
            result = await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove the roles");
            return Ok(await _userManager.GetRolesAsync(user));

        }

    }
}
