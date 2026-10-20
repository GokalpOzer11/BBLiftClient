namespace BBLiftClient.Application.Dtos;

public record UserProfileResponse(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    DateOnly DateOfBirth,
    decimal Weight,
    decimal Height,
    int TotalWorkouts,  
    decimal TotalVolume, 
    List<RecentWorkoutResponse> RecentWorkouts 
);