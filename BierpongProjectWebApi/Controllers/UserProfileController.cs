using BierpongProjectWebApi.Models.Entities;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly UserProfileService _userProfileService;

    public UserProfileController(UserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    // Get User Profile
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

    // Update User Profile
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UserProfile request)
    {
        var updatedProfile = await _userProfileService.UpdateUserProfileAsync(userId, request.Name, request.Bio, request.ProfilePictureUrl);

        if (updatedProfile == null)
        {
            return NotFound("User profile not found.");
        }

        return Ok(updatedProfile);
    }

    // Add Friend
    [HttpPost("{userId}/addFriend/{friendUserId}")]
    public async Task<IActionResult> AddFriend(Guid userId, Guid friendUserId)
    {
        var result = await _userProfileService.AddFriendAsync(userId, friendUserId);

        if (!result)
        {
            return BadRequest("Friendship already exists or invalid request.");
        }

        return Ok("Friend request sent.");
    }

    // Accept Friend Request
    [HttpPost("{userId}/acceptFriendRequest/{friendUserId}")]
    public async Task<IActionResult> AcceptFriendRequest(Guid userId, Guid friendUserId)
    {
        var result = await _userProfileService.AcceptFriendRequestAsync(userId, friendUserId);

        if (!result)
        {
            return BadRequest("Friend request not found or already accepted.");
        }

        return Ok("Friend request accepted.");
    }

    // Reject or Block Friend Request
    [HttpPost("{userId}/rejectOrBlockFriendRequest/{friendUserId}")]
    public async Task<IActionResult> RejectOrBlockFriendRequest(Guid userId, Guid friendUserId, [FromQuery] bool isBlock)
    {
        var result = await _userProfileService.RejectOrBlockFriendRequestAsync(userId, friendUserId, isBlock);

        if (!result)
        {
            return BadRequest("Friend request not found.");
        }

        return Ok(isBlock ? "Friend request blocked." : "Friend request rejected.");
    }

    // Get User's Friends
    [HttpGet("{userId}/friends")]
    public async Task<IActionResult> GetUserFriends(Guid userId)
    {
        var friends = await _userProfileService.GetUserFriendsAsync(userId);

        return Ok(friends);
    }
}
