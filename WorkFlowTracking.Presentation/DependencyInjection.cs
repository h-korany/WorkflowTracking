using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowTracking.Application.Interfaces;
using WorkFlowTracking.Application.Services;
using WorkFlowTracking.Domain.Interfaces;
using WorkFlowTracking.Infrastructure.Repositories;
using WorkFlowTracking.Infrastucture.Data;

namespace WorkFlowTracking.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WorkflowTrackingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        // Register services
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IProcessService, ProcessService>();
        services.AddScoped<IValidationService, ValidationService>();

        // Configure HTTP client with retry policy
        services.AddHttpClient("ValidationApi", client =>
        {
            client.BaseAddress = new Uri(configuration["ValidationApi:BaseUrl"] ?? "https://localhost:7001");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient();

        return services;
    }
}