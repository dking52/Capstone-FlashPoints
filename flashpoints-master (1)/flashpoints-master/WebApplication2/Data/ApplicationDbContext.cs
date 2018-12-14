using System;
using System.Collections.Generic;
using System.Text;
using FlashPoints.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashPoints.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Prize> Prize { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PrizeRedeemed>()
                .HasKey(pr => new { pr.UserID, pr.PrizeID });

            modelBuilder.Entity<PrizeRedeemed>()
                .HasOne(bc => bc.Prize)
                .WithMany(b => b.PrizesRedeemed)
                .HasForeignKey(bc => bc.PrizeID);

            modelBuilder.Entity<PrizeRedeemed>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.PrizesRedeemed)
                .HasForeignKey(bc => bc.UserID);



            modelBuilder.Entity<EventAttended>()
                .HasKey(pr => new { pr.UserID, pr.EventID });

            modelBuilder.Entity<EventAttended>()
                .HasOne(bc => bc.Event)
                .WithMany(b => b.EventsAttended)
                .HasForeignKey(bc => bc.EventID);

            modelBuilder.Entity<EventAttended>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.EventsAttended)
                .HasForeignKey(bc => bc.UserID);
        }
    }
}
