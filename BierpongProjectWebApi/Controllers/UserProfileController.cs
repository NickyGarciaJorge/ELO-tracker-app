using BierpongProjectWebApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Ensures all actions require authentication
public class UserProfileController : ControllerBase
{
    private readonly UserProfileService _userProfileService;

    public UserProfileController(UserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    // Get User Profile (Publicly Accessible)
    [AllowAnonymous]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync(userId);
        if (userProfile == null)
        {
            return NotFound("User profile not found.");
        }
        return Ok(userProfile);
    }

    // Update User Profile (Only Owner or Admin)
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UserProfile request)
    {
        if (!IsAuthorizedUser(userId))
        {
            return Forbid();
        }

        var updatedProfile = await _userProfileService.UpdateUserProfileAsync(userId, request.Name, request.Bio, request.ProfilePictureUrl);
        if (updatedProfile == null)
        {
            return NotFound("User profile not found.");
        }
        return Ok(updatedProfile);
    }

    // Add Friend (Only Self or Admin & Users Not Already Friends)
    [HttpPost("{userId}/addFriend/{friendUserId}")]
    public async Task<IActionResult> AddFriend(Guid userId, Guid friendUserId)
    {
        if (!IsAuthorizedUser(userId))
        {
            return Forbid();
        }

        bool areFriends = await _userProfileService.AreFriendsAsync(userId, friendUserId);
        if (areFriends)
        {
            return BadRequest("Users are already friends.");
        }

        var result = await _userProfileService.AddFriendAsync(userId, friendUserId);
        return result ? Ok("Friend request sent.") : BadRequest("Friend request failed.");
    }

    // Accept Friend Request (Only Self or Admin)
    [HttpPost("{userId}/acceptFriendRequest/{friendUserId}")]
    public async Task<IActionResult> AcceptFriendRequest(Guid userId, Guid friendUserId)
    {
        if (!IsAuthorizedUser(userId))
        {
            return Forbid();
        }

        var result = await _userProfileService.AcceptFriendRequestAsync(userId, friendUserId);
        return result ? Ok("Friend request accepted.") : BadRequest("Friend request not found.");
    }

    // Reject or Block Friend Request (Only Self or Admin)
    [HttpPost("{userId}/rejectOrBlockFriendRequest/{friendUserId}")]
    public async Task<IActionResult> RejectOrBlockFriendRequest(Guid userId, Guid friendUserId, [FromQuery] bool isBlock)
    {
        if (!IsAuthorizedUser(userId))
        {
            return Forbid();
        }

        var result = await _userProfileService.RejectOrBlockFriendRequestAsync(userId, friendUserId, isBlock);
        return result ? Ok(isBlock ? "Friend request blocked." : "Friend request rejected.") : BadRequest("Friend request not found.");
    }

    // Get User's Friends (Publicly Accessible)
    [AllowAnonymous]
    [HttpGet("{userId}/friends")]
    public async Task<IActionResult> GetUserFriends(Guid userId)
    {
        var friends = await _userProfileService.GetUserFriendsAsync(userId);
        return Ok(friends);
    }

    [HttpGet("{userId}/friends/pending")]
    public async Task<IActionResult> GetPendingFriendRequests(Guid userId)
    {
        if (!IsAuthorizedUser(userId))
        {
            return Forbid();
        }
        var pendingRequests = await _userProfileService.GetPendingFriendRequestsAsync(userId);
        return Ok(pendingRequests);
    }

    [HttpDelete("{userId}/friends")]
    public async Task<IActionResult> DeleteFriend(Guid userId, Guid friendUserId)
    {
        if (!IsAuthorizedUser(userId))
        {
            return Forbid();
        }
        var result = await _userProfileService.RemoveFriendAsync(userId, friendUserId);
        return result ? Ok("Friend deleted.") : BadRequest("Failed to delete friend.");
    }

    [HttpGet("{userid}/matchHistory")]
    public async Task<IActionResult> GetMatchHistory(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        // Check if the user is authorized
        if (!IsAuthorizedUser(userId))
        {
            return Forbid();
        }

        // Call the service to get paginated match history
        var matchHistory = await _userProfileService.GetMatchHistoryAsync(userId, pageNumber, pageSize);

        // If no match history is found, return a NotFound response
        if (matchHistory == null || !matchHistory.Any())
        {
            return NotFound("Match history not found.");
        }

        // Return a paginated response
        return Ok(matchHistory);
    }

    // Helper: Check if user is owner or admin
    private bool IsAuthorizedUser(Guid userId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");
        return isAdmin || (currentUserId != null && Guid.Parse(currentUserId) == userId);
    }
}
