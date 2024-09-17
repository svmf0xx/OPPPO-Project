using Microsoft.EntityFrameworkCore;
using Shcheduler.Core.DAL.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL
{
    public class ShchedulerContext : DbContext
    {
        public ShchedulerContext(DbContextOptions<ShchedulerContext> options):base(options) {
            Database.EnsureCreated();
        }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<RawResponseModel> RawResponses { get; set; }
        public DbSet<ProcessedResponseModel> ProcessedResponses { get; set; }
        public DbSet<AgentModel> Agents { get; set; }
        public DbSet<JobModel> Jobs { get; set; }
        public DbSet<TaskResponseModel> TaskResponses { get; set; }
        public DbSet<JobResponseModel> JobResponses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobResponseModel>()
            .HasOne(j => j.TaskResponse)
            .WithMany(t => t.JobResponses)
            .HasForeignKey(j => j.TaskResponseId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
