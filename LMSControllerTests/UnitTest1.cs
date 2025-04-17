using LMS.Controllers;
using LMS.Models.LMSModels;
using LMS_CustomIdentity.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LMSControllerTests
{
    public class UnitTest1
    {
        // Uncomment the methods below after scaffolding
        // (they won't compile until then)

        [Fact]
        public void Test1()
        {
           // An example of a simple unit test on the CommonController
           CommonController ctrl = new CommonController(MakeTinyDB());

           var allDepts = ctrl.GetDepartments() as JsonResult;

           dynamic x = allDepts.Value;

           Assert.Equal( 1, x.Length );
           Assert.Equal( "CS", x[0].subject );
        }


        /// <summary>
        /// Make a very tiny in-memory database, containing just one department
        /// and nothing else.
        /// </summary>
        /// <returns></returns>
        LMSContext MakeTinyDB()
        {
           var contextOptions = new DbContextOptionsBuilder<LMSContext>()
           .UseInMemoryDatabase( "LMSControllerTest" )
           .ConfigureWarnings( b => b.Ignore( InMemoryEventId.TransactionIgnoredWarning ) )
           .UseApplicationServiceProvider( NewServiceProvider() )
           .Options;

           var db = new LMSContext(contextOptions);

           db.Database.EnsureDeleted();
           db.Database.EnsureCreated();

           db.Departments.Add( new Department { Name = "KSoC", Subject = "CS" } );

           // TODO: add more objects to the test database

           db.SaveChanges();

           return db;
        }

        private static ServiceProvider NewServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
          .AddEntityFrameworkInMemoryDatabase()
          .BuildServiceProvider();

            return serviceProvider;
        }

        [Fact]
        public void GradeSubmission_RecalculatesEnrollmentGrade()
        {

            var options = new DbContextOptionsBuilder<LMSContext>()
                .UseInMemoryDatabase("GradeSubmissionTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseApplicationServiceProvider(NewServiceProvider())
                .Options;

            var db = new LMSContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();


            db.Departments.Add(new Department { Subject = "CS", Name = "CompSci" });
            db.Courses     .Add(new Course     { CourseId = 1, Subject = "CS", Number = 101, Name = "Intro" });
            db.Classes     .Add(new Class      { ClassId = 1, CourseId = 1, Season = "Spring", Year = 2025, Location = "R101", StartTime = TimeOnly.FromTimeSpan(TimeSpan.Zero), EndTime = TimeOnly.FromTimeSpan(TimeSpan.Zero), ProfessorUid = "p0000001" });
            db.AssignmentCategories.Add(new AssignmentCategory { AcId = 1, ClassId = 1, Name = "HW", Weight = 10 });
            db.Assignments .Add(new Assignment { AId = 1, AcId = 1, Name = "A1", Points = 100, Due = DateTime.Now, Contents = "" });
            db.Students    .Add(new Student    { UId = "u0000001", FName = "Stu", LName = "Dent", Dob = DateOnly.FromDateTime(DateTime.Now), Subject = "CS" });
            db.Enrollments .Add(new Enrollment { Student = "u0000001", ClassId = 1, Grade = "--" });
            db.Submissions .Add(new Submission { SId = "u0000001", AId = 1, Score = 100, Time = DateTime.Now, Contents = "" });

            db.SaveChanges();

            var ctrl = new ProfessorController(db);


            var result = ctrl.GradeSubmission(
                subject:  "CS",
                num:       101,
                season:   "Spring",
                year:      2025,
                category:  "HW",
                asgname:   "A1",
                uid:       "u0000001",
                score:     100
            ) as JsonResult;
            Assert.NotNull(result);
            Assert.True(((dynamic)result.Value).success);

            var enrollment = db.Enrollments.First(e => e.Student == "u0000001" && e.ClassId == 1);
            Assert.Equal("A", enrollment.Grade);
        }

        [Fact]
        public void CreateAssignment_RecalculatesGradesForAllStudents()
        {
            var options = new DbContextOptionsBuilder<LMSContext>()
                .UseInMemoryDatabase("CreateAssignmentTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseApplicationServiceProvider(NewServiceProvider())
                .Options;

            var db = new LMSContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departments.Add(new Department { Subject = "CS", Name = "CompSci" });
            db.Courses     .Add(new Course     { CourseId = 2, Subject = "CS", Number = 201, Name = "DataStruct" });
            db.Classes     .Add(new Class      { ClassId = 2, CourseId = 2, Season = "Fall",  Year = 2025, Location = "R202", StartTime = TimeOnly.FromTimeSpan(TimeSpan.Zero), EndTime = TimeOnly.FromTimeSpan(TimeSpan.Zero), ProfessorUid = "p0000001" });
            db.AssignmentCategories.Add(new AssignmentCategory { AcId = 2, ClassId = 2, Name = "Lab", Weight = 20 });
            db.Students    .Add(new Student    { UId = "u0000002", FName = "Alice", LName = "Smith", Dob = DateOnly.FromDateTime(DateTime.Now), Subject = "CS" });
            db.Enrollments .Add(new Enrollment { Student = "u0000002", ClassId = 2, Grade = "--" });

            db.SaveChanges();

            var ctrl = new ProfessorController(db);
            var result = ctrl.CreateAssignment(
                subject:    "CS",
                num:         201,
                season:    "Fall",
                year:       2025,
                category:    "Lab",
                asgname:     "L1",
                asgpoints:    50,
                asgdue:     DateTime.Now,
                asgcontents: "Do this lab"
            ) as JsonResult;
            Assert.NotNull(result);
            Assert.True(((dynamic)result.Value).success);
            var enrollment = db.Enrollments.First(e => e.Student == "u0000002" && e.ClassId == 2);
            Assert.Equal("E", enrollment.Grade);
        }

    }
}