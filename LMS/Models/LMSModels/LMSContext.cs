using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Enrollment> Enrollments { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.8-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.AcId, e.Name }, "acID")
                    .IsUnique();

                entity.Property(e => e.AId)
                    .HasColumnType("int(11)")
                    .HasColumnName("aID");

                entity.Property(e => e.AcId)
                    .HasColumnType("int(11)")
                    .HasColumnName("acID");

                entity.Property(e => e.Contents)
                    .HasColumnType("text")
                    .HasColumnName("contents");

                entity.Property(e => e.Due)
                    .HasColumnType("datetime")
                    .HasColumnName("due");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Points)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("points");

                entity.HasOne(d => d.Ac)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.AcId)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.AcId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.ClassId, e.Name }, "classID")
                    .IsUnique();

                entity.Property(e => e.AcId)
                    .HasColumnType("int(11)")
                    .HasColumnName("acID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11)")
                    .HasColumnName("classID");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Weight)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("weight");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => e.CourseId, "courseID");

                entity.HasIndex(e => e.ProfessorUid, "professorUID");

                entity.HasIndex(e => new { e.Year, e.Season, e.CourseId }, "year")
                    .IsUnique();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11)")
                    .HasColumnName("classID");

                entity.Property(e => e.CourseId)
                    .HasColumnType("int(11)")
                    .HasColumnName("courseID");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time")
                    .HasColumnName("endTime");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.ProfessorUid)
                    .HasMaxLength(8)
                    .HasColumnName("professorUID")
                    .IsFixedLength();

                entity.Property(e => e.Season)
                    .HasColumnType("enum('Spring','Summer','Fall')")
                    .HasColumnName("season");

                entity.Property(e => e.StartTime)
                    .HasColumnType("time")
                    .HasColumnName("startTime");

                entity.Property(e => e.Year)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("year");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("Classes_ibfk_1");

                entity.HasOne(d => d.ProfessorU)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ProfessorUid)
                    .HasConstraintName("Classes_ibfk_2");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => new { e.Number, e.Subject }, "number")
                    .IsUnique();

                entity.HasIndex(e => e.Subject, "subject");

                entity.Property(e => e.CourseId)
                    .HasColumnType("int(11)")
                    .HasColumnName("courseID");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .HasColumnType("int(11)")
                    .HasColumnName("number");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Subject)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.Student, e.ClassId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("Enrollment");

                entity.HasIndex(e => e.ClassId, "classID");

                entity.Property(e => e.Student)
                    .HasMaxLength(8)
                    .HasColumnName("student")
                    .IsFixedLength();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11)")
                    .HasColumnName("classID");

                entity.Property(e => e.Grade)
                    .HasMaxLength(2)
                    .HasColumnName("grade");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("Enrollment_ibfk_2");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.Student)
                    .HasConstraintName("Enrollment_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject, "subject");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Subject)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject, "subject");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Subject)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.SId, e.AId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.AId, "aID");

                entity.Property(e => e.SId)
                    .HasMaxLength(8)
                    .HasColumnName("sID")
                    .IsFixedLength();

                entity.Property(e => e.AId)
                    .HasColumnType("int(11)")
                    .HasColumnName("aID");

                entity.Property(e => e.Contents)
                    .HasColumnType("text")
                    .HasColumnName("contents");

                entity.Property(e => e.Score)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("score");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("time");

                entity.HasOne(d => d.AIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AId)
                    .HasConstraintName("Submissions_ibfk_1");

                entity.HasOne(d => d.SIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.SId)
                    .HasConstraintName("Submissions_ibfk_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
