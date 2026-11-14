using Meaar5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace Meaar5.Data


{
    public class ApplicationDbContext : DbContext
    {



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CourcesData> Courses { get; set; }

       public DbSet<FacultyMembers> FacultyMembers { get; set; }
        public DbSet<FacultyCources> FacultyCources { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<DepartmentHead> DepartmentHeads { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Grades> Grades { get; set; }
        public DbSet<CLO> CLO { get; set; }
        public DbSet<CourseCLO> CourseCLO { get; set; }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<FacultyCources>()
                .HasOne(fc => fc.FacultyMember)
                .WithMany()
                .HasForeignKey(fc => fc.FacultyId);

            modelBuilder.Entity<FacultyCources>()
                .HasOne(fc => fc.Course)
                .WithMany()
                .HasForeignKey(fc => fc.CourseId);
            
        }

      





    }

}