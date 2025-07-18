using Freelancer_Task.Data;
using Freelancer_Task.Forms;
using Freelancer_Task.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace Freelancer_Task
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            ConfigureServices(services);

            using var provider = services.BuildServiceProvider();
            var mainForm = provider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        static void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<FreelancerContext>();

            // Repositories
            services.AddScoped<ClientRepository>();
            services.AddScoped<ProjectRepository>();
            services.AddScoped<TaskRepository>();

            // Forms
            services.AddTransient<MainForm>();
            services.AddTransient<ClientForm>();
            services.AddTransient<ProjectForm>();
            services.AddTransient<TaskForm>();
        }
    }
}