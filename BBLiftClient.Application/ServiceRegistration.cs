
using BBLiftClient.Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace BBLiftClient.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<CartService>();
        services.AddScoped<EquipmentService>();
        services.AddScoped<ExerciseService>();
        services.AddScoped<FavoriteExerciseService>();
        services.AddScoped<MembershipPackageService>(); 
        services.AddScoped<MuscleGroupService>();
        services.AddScoped<OrderService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<RoutineExerciseService>();
        services.AddScoped<RoutineService>();
        services.AddScoped<WorkoutLogService>();
        services.AddScoped<WorkoutSessionService>();
        services.AddScoped<UserService>();

        return services;
    }
}