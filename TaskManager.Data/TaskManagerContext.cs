using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data
{
    public class TaskManagerContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                    
                   .SetBasePath(AppContext.BaseDirectory)
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .Build();

            var connectionString = configuration["ConnectionStrings:DefaultConnection"];
            optionsBuilder.UseSqlServer(connectionString);
        }

        public TaskManagerContext() { }

        public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
        : base(options)
        { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<TaskCapacity> TaskCapacities { get; set; }
        public DbSet<TaskEmployee> TaskEmployees { get; set; }
        public DbSet<TaskFollowUp> TaskFollowUps { get; set; }
        public DbSet<TaskFollowUpResponse> TaskFollowUpResponses { get; set; }
        public DbSet<TaskPriority> TaskPriority { get; set; }
        public DbSet<TaskStatus> TaskStatus { get; set; }
        public DbSet<TaskAudit> TaskAudit { get; set; }
        public DbSet<AuditType> AuditType { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskFollowUpStatus> TaskFollowUpStatus { get; set; }

    }
}
