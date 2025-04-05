using BierpongProjectWebApi.Data;
using BierpongProjectWebApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

public class UserProfileService
{
    private readonly CustomDbContext _dbContext;

    public UserProfileService()
    {
        
    }
    public UserProfileService(CustomDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Get User Profile by UserId
    public virtual async Task<UserProfile> GetUserProfileAsync(Guid userId)
    {
        return await _dbContext.UserProfiles
            .Include(up => up.Friendships)
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }


    // Update User Profile
    public virtual async Task<UserProfile> UpdateUserProfileAsync(Guid userId, string name, string bio, string profilePictureUrl)
    {
        var userProfile = await _dbContext.UserProfiles.FindAsync(userId);

        if (userProfile != null)
        {
            userProfile.Name = name;
            userProfile.Bio = bio;
            userProfile.ProfilePictureUrl = profilePictureUrl;

            await _dbContext.SaveChangesAsync();
        }

        return userProfile;
    }

    // Add Friend
    public virtual async Task<bool> AddFriendAsync(Guid userId, Guid friendUserId)
    {
        var existingFriendship = await _dbContext.Friendships
        .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendUserId == friendUserId) || (f.UserId == friendUserId && f.FriendUserId == userId));

        if (existingFriendship != null)
        {
            return false;  // Friendship already exists, return false
        }

        var friendship = new Friendship
        {
            UserId = userId,
            FriendUserId = friendUserId,
            Status = FriendshipStatus.Pending,
            DateRequested = DateTime.UtcNow
        };

        _dbContext.Friendships.Add(friendship);
        await _dbContext.SaveChangesAsync();

        return true;
    }
    public virtual async Task<bool> AreFriendsAsync(Guid userId, Guid friendUserId)
    {
        bool areFriends = await _dbContext.Friendships
            .AnyAsync(f => (f.UserId == userId && f.FriendUserId == friendUserId) || (f.UserId == friendUserId && f.FriendUserId == userId));

        if (areFriends)
        {
            return true;  // Users are friends
        }
        else
        {
            areFriends = await _dbContext.Friendships
            .AnyAsync(f => ((f.UserId == userId && f.FriendUserId == friendUserId) || (f.UserId == friendUserId && f.FriendUserId == userId))
                       && f.Status == FriendshipStatus.Pending);
        }

        return areFriends;  // Users are not friends
    }

    // Accept Friend Request
    public virtual async Task<bool> AcceptFriendRequestAsync(Guid userId, Guid friendUserId)
    {
        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.UserId == friendUserId && f.FriendUserId == userId && f.Status == FriendshipStatus.Pending);

        if (friendship == null)
        {
            return false;  // No pending friend request
        }

        friendship.Status = FriendshipStatus.Accepted;
        friendship.DateAccepted = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    // Reject or Block Friend Request
    public virtual async Task<bool> RejectOrBlockFriendRequestAsync(Guid userId, Guid friendUserId, bool isBlock)
    {
        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => f.UserId == friendUserId && f.FriendUserId == userId && f.Status == FriendshipStatus.Pending);

        if (friendship == null)
        {
            return false;  // No pending friend request
        }

        friendship.Status = isBlock ? FriendshipStatus.Blocked : FriendshipStatus.Rejected;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    // Get All Friends of a User
    public virtual async Task<List<UserProfile>> GetUserFriendsAsync(Guid userId)
    {
        var friendships = await _dbContext.Friendships
            .Where(f => (f.UserId == userId || f.FriendUserId == userId) && f.Status == FriendshipStatus.Accepted)
            .ToListAsync();

        var friendIds = friendships.Select(f => f.UserId == userId ? f.FriendUserId : f.UserId).ToList();

        return await _dbContext.UserProfiles
            .Where(up => friendIds.Contains(up.UserId))
            .ToListAsync();
    }

    public virtual async Task<List<UserProfile>> GetPendingFriendRequestsAsync(Guid userId)
    {
        var pendingRequests = await _dbContext.Friendships
            .Where(f => f.FriendUserId == userId && f.Status == FriendshipStatus.Pending)
            .ToListAsync();
        var requestIds = pendingRequests.Select(f => f.UserId).ToList();
        return await _dbContext.UserProfiles
            .Where(up => requestIds.Contains(up.UserId))
            .ToListAsync();
    }

    public virtual async Task<bool> RemoveFriendAsync(Guid userId, Guid friendUserId)
    {
        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendUserId == friendUserId) || (f.UserId == friendUserId && f.FriendUserId == userId));
        if (friendship != null)
        {
            _dbContext.Friendships.Remove(friendship);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;  // Friendship not found
    }

    public virtual async Task<List<MatchHistory>> GetMatchHistoryAsync(Guid userId, int pageNumber, int pageSize)
    {
        // Calculate the starting index of the records to fetch
        var skip = (pageNumber - 1) * pageSize;

        // Fetch paginated match history from the database
        var matchHistory = await _dbContext.MatchHistories
            .Where(m => m.PlayerId == userId)
            .OrderByDescending(m => m.Date) // Assuming you want to order by date, you can change this
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return matchHistory;
    }

}
