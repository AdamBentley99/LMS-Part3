using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo("LMSControllerTests")]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var cls = (from c in db.Classes
                       join crs in db.Courses on c.CourseId equals crs.CourseId
                       where crs.Subject == subject
                          && crs.Number == num
                          && c.Season == season
                          && c.Year == year
                       select c)
             .FirstOrDefault();
            if (cls == null)
                return Json(new object[0]);

            var result = (from e in db.Enrollments
                          where e.ClassId == cls.ClassId
                          join st in db.Students on e.Student equals st.UId
                          select new
                          {
                              fname = st.FName,
                              lname = st.LName,
                              uid = st.UId,
                              dob = st.Dob,
                              grade = e.Grade
                          })
                         .ToArray();

            return Json(result);
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var cls = (from c in db.Classes
                       join crs in db.Courses on c.CourseId equals crs.CourseId
                       where crs.Subject == subject
                          && crs.Number == num
                          && c.Season == season
                          && c.Year == year
                       select c).FirstOrDefault();

            if (cls == null)
                return Json(new object[0]);

            // Query only the base assignment info from EF
            var baseQuery = (from ac in db.AssignmentCategories
                             where ac.ClassId == cls.ClassId
                                && (category == null || ac.Name == category)
                             join a in db.Assignments on ac.AcId equals a.AcId
                             select new
                             {
                                 a.AId,
                                 aname = a.Name,
                                 cname = ac.Name,
                                 due = a.Due
                             }).ToList();

            var result = baseQuery.Select(a => new
            {
                aname = a.aname,
                cname = a.cname,
                due = a.due,
                submissions = db.Submissions.Count(s => s.AId == a.AId)
            }).ToArray();

            return Json(result);
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var cls = (from c in db.Classes
                       join crs in db.Courses on c.CourseId equals crs.CourseId
                       where crs.Subject == subject
                          && crs.Number == num
                          && c.Season == season
                          && c.Year == year
                       select c)
             .FirstOrDefault();
            if (cls == null)
                return Json(new object[0]);

            var result = db.AssignmentCategories
                           .Where(ac => ac.ClassId == cls.ClassId)
                           .Select(ac => new
                           {
                               name = ac.Name,
                               weight = ac.Weight
                           })
                           .ToArray();

            return Json(result);
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var cls = (from c in db.Classes
                       join crs in db.Courses on c.CourseId equals crs.CourseId
                       where crs.Subject == subject
                          && crs.Number == num
                          && c.Season == season
                          && c.Year == year
                       select c)
             .FirstOrDefault();
            if (cls == null)
                return Json(new { success = false });

            bool exists = db.AssignmentCategories
                            .Any(ac => ac.ClassId == cls.ClassId
                                    && ac.Name == category);
            if (exists)
                return Json(new { success = false });

            var cat = new AssignmentCategory
            {
                ClassId = cls.ClassId,
                Name = category,
                Weight = (uint)catweight
            };
            db.AssignmentCategories.Add(cat);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var cls = (from c in db.Classes
                       join crs in db.Courses on c.CourseId equals crs.CourseId
                       where crs.Subject == subject
                          && crs.Number == num
                          && c.Season == season
                          && c.Year == year
                       select c)
             .FirstOrDefault();
            if (cls == null)
                return Json(new { success = false });

            var ac = db.AssignmentCategories
                       .FirstOrDefault(x => x.ClassId == cls.ClassId
                                         && x.Name == category);
            if (ac == null)
                return Json(new { success = false });

            bool duplicate = db.Assignments
                               .Any(a => a.AcId == ac.AcId
                                      && a.Name == asgname);
            if (duplicate)
                return Json(new { success = false });

            var a = new Assignment
            {
                AcId = ac.AcId,
                Name = asgname,
                Points = (uint)asgpoints,
                Due = asgdue,
                Contents = asgcontents
            };
            db.Assignments.Add(a);
            db.SaveChanges();

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var cls = (from c in db.Classes
                       join crs in db.Courses on c.CourseId equals crs.CourseId
                       where crs.Subject == subject
                          && crs.Number == num
                          && c.Season == season
                          && c.Year == year
                       select c)
              .FirstOrDefault();
            if (cls == null)
                return Json(new object[0]);

            var ac = db.AssignmentCategories
                       .FirstOrDefault(x => x.ClassId == cls.ClassId
                                         && x.Name == category);
            if (ac == null)
                return Json(new object[0]);

            var asg = db.Assignments
                        .FirstOrDefault(a => a.AcId == ac.AcId
                                          && a.Name == asgname);
            if (asg == null)
                return Json(new object[0]);

            var result = (from s in db.Submissions
                          where s.AId == asg.AId
                          join st in db.Students on s.SId equals st.UId
                          select new
                          {
                              fname = st.FName,
                              lname = st.LName,
                              uid = st.UId,
                              time = s.Time,
                              score = s.Score
                          })
                         .ToArray();

            return Json(result);
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var sub = (from s in db.Submissions
                       join a in db.Assignments
                         on s.AId equals a.AId
                       join ac in db.AssignmentCategories
                         on a.AcId equals ac.AcId
                       join cl in db.Classes
                         on ac.ClassId equals cl.ClassId
                       join cr in db.Courses
                         on cl.CourseId equals cr.CourseId
                       where s.SId == uid
                          && a.Name == asgname
                          && ac.Name == category
                          && cl.Season == season
                          && cl.Year == year
                          && cr.Number == num
                          && cr.Subject == subject
                       select s)
              .FirstOrDefault();

            if (sub == null)
                return Json(new { success = false });

            // update the score
            sub.Score = (uint)score;
            db.SaveChanges();
            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var result = (from cls in db.Classes
                          where cls.ProfessorUid == uid
                          join crs in db.Courses on cls.CourseId equals crs.CourseId
                          select new
                          {
                              subject = crs.Subject,
                              number = crs.Number,
                              name = crs.Name,
                              season = cls.Season,
                              year = cls.Year
                          })
                 .ToArray();

            return Json(result);
        }



        /*******End code to modify********/
    }
}

