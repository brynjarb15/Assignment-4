using System;
using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;
using CoursesAPI.Services.CoursesServices;
using CoursesAPI.Tests.MockObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoursesAPI.Tests.Services
{
	[TestClass]
    public class CourseServicesTests
	{
		private MockUnitOfWork<MockDataContext> _mockUnitOfWork;
		private CoursesServiceProvider _service;
		private List<TeacherRegistration> _teacherRegistrations;

		private const string SSN_DABS    = "1203735289";
		private const string SSN_GUNNA   = "1234567890";
		private const string INVALID_SSN = "9876543210";

		private const string NAME_GUNNA  = "Guðrún Guðmundsdóttir";
		private const string NAME_DABS = "Daníel B. Sigurgeirsson";
		private const int COURSEID_VEFT_20153 = 1337;
		private const int COURSEID_VEFT_20163 = 1338;
		private const int COURSEID_BATM_20163 = 1339;
		private const int COURSEID_VEFT_20143 = 1340;
		private const int COURSEID_BATM_20143 = 1341;
		private const int INVALID_COURSEID    = 9999;

		[TestInitialize]
		public void CourseServicesTestsSetup()
		{
			_mockUnitOfWork = new MockUnitOfWork<MockDataContext>();

			#region Persons
			var persons = new List<Person>
			{
				// Of course I'm the first person,
				// did you expect anything else?
				new Person
				{
					ID    = 1,
					Name  = NAME_DABS,
					SSN   = SSN_DABS,
					Email = "dabs@ru.is"
				},
				new Person
				{
					ID    = 2,
					Name  = NAME_GUNNA,
					SSN   = SSN_GUNNA,
					Email = "gunna@ru.is"
				}
			};
			#endregion

			#region Course templates

			var courseTemplates = new List<CourseTemplate>
			{
				new CourseTemplate
				{
					CourseID    = "T-514-VEFT",
					Description = "Í þessum áfanga verður fjallað um vefþj...",
					Name        = "Vefþjónustur"
				},
				new CourseTemplate
				{
					CourseID = "T-313-BATM",
					Description = "Learn to fight crimes",
					Name = "Crime fighting"
				}
			};
			#endregion

			#region Courses
			var courses = new List<CourseInstance>
			{
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20153,
					CourseID   = "T-514-VEFT",
					SemesterID = "20153"
				},
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20163,
					CourseID   = "T-514-VEFT",
					SemesterID = "20163"
				},
				new CourseInstance
				{
					ID = COURSEID_BATM_20163,
					CourseID = "T-313-BATM",
					SemesterID = "20163"
				},
				new CourseInstance
				{
					ID = COURSEID_BATM_20143,
					CourseID = "T-313-BATM",
					SemesterID = "20143"
				},
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20143,
					CourseID   = "T-514-VEFT",
					SemesterID = "20143"
				}
			};
			#endregion

			#region Teacher registrations
			_teacherRegistrations = new List<TeacherRegistration>
			{
				new TeacherRegistration
				{
					ID               = 101,
					CourseInstanceID = COURSEID_VEFT_20153,
					SSN              = SSN_DABS,
					Type             = TeacherType.MainTeacher
				},
				new TeacherRegistration
				{
					ID = 102,
					CourseInstanceID = COURSEID_VEFT_20153,
					SSN = SSN_GUNNA,
					Type = TeacherType.AssistantTeacher
				},
				new TeacherRegistration
				{
					ID               = 103,
					CourseInstanceID = COURSEID_BATM_20143,
					SSN              = SSN_DABS,
					Type             = TeacherType.MainTeacher
				},
				new TeacherRegistration
				{
					ID               = 104,
					CourseInstanceID = COURSEID_VEFT_20143,
					SSN              = SSN_GUNNA,
					Type             = TeacherType.MainTeacher
				}
				
			};
			#endregion

			_mockUnitOfWork.SetRepositoryData(persons);
			_mockUnitOfWork.SetRepositoryData(courseTemplates);
			_mockUnitOfWork.SetRepositoryData(courses);
			_mockUnitOfWork.SetRepositoryData(_teacherRegistrations);

			// TODO: this would be the correct place to add
			// more mock data to the mockUnitOfWork!

			_service = new CoursesServiceProvider(_mockUnitOfWork);
		}

		#region GetCoursesBySemester
		/// <summary>
		/// Tests if we return an empty list if there is no data for the given semester
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_ReturnsEmptyListWhenNoDataDefined()
		{
			// Arrange:
			var semesterWithNoCourse = "20173";
			// Act:
			var result = _service.GetCourseInstancesBySemester(semesterWithNoCourse);
			// Assert:
			Assert.IsTrue(result.Count == 0);
		}

		/// <summary>
		/// Checks if there are 2 courses in the semester 20163
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_ReturnsTwoCourseForSemester20163()
		{
			// Arrange:
			// Act:
			var result = _service.GetCourseInstancesBySemester("20163");
			// Assert:
			Assert.IsTrue(result.Count == 2);
		}
		/// <summary>
		/// Checks the number of courses for the semester 20153
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_GetTwoCoursesForSemester20153()
		{
			// Arrange:
			// Act:
			var result = _service.GetCourseInstancesBySemester("20153");
			// Assert:
			Assert.IsTrue(result.Count == 1);
		}

		/// <summary>
		/// Checks if there are 2 courses in the semester 20143
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_ReturnsTwoCourseForSemester20143()
		{
			// Arrange:
			// Act:
			var result = _service.GetCourseInstancesBySemester("20143");
			// Assert:
			Assert.IsTrue(result.Count == 2);
		}
		/// <summary>
		/// Checks if the course returned if nothing is sent in is Vefþjónustur which is the only
		/// course in the semester 20153
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_ReturnsVefthjonusturNothingIsSentIn()
		{
			// Arrange:
			// Act:
			var result = _service.GetCourseInstancesBySemester();
			// Assert:
			Assert.IsTrue(result.Count == 1);
			Assert.AreEqual(result[0].Name, "Vefþjónustur");
			Assert.AreEqual(result[0].TemplateID, "T-514-VEFT");
			Assert.AreEqual(result[0].CourseInstanceID, COURSEID_VEFT_20153);
		}
		/// <summary>
		/// Checks if the main techer in the only course in the semester 20153
		/// is Dabs
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_ReturnsDABSAsMainTeacher()
		{
			// Arrange:
			// Act:
			var result = _service.GetCourseInstancesBySemester("20153");
			// Assert:
			Assert.AreEqual(result[0].MainTeacher, NAME_DABS);
		}
		/// <summary>
		/// No main teachers have been registered to the courses of semester 20163
		/// so all the teachers name should be ""
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_AllTeacherShouldBeEmpty()
		{
			// Arrange:
			// Act:
			var result = _service.GetCourseInstancesBySemester("20163");
			// Assert:
			foreach(var c in result)
			{
				Assert.AreEqual(c.MainTeacher, "");
			}
		}
		/// <summary>
		/// Check if it returns different teacher name for different courses
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_GetDifferentMainTeacherForDifferentCourses()
		{
			// Arrange:
			// Act:
			var result = _service.GetCourseInstancesBySemester("20143");
			// Assert:
			Assert.AreEqual(result[0].MainTeacher, NAME_DABS);
			Assert.AreEqual(result[1].MainTeacher, NAME_GUNNA);
		}




		#endregion

		#region AddTeacher

		/// <summary>
		/// Adds a main teacher to a course which doesn't have a
		/// main teacher defined already (see test data defined above).
		/// </summary>
		[TestMethod]
		public void AddTeacher_WithValidTeacherAndCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};
			var prevCount = _teacherRegistrations.Count;

			// Act:
			var dto = _service.AddTeacherToCourse(COURSEID_VEFT_20163, model);

			// Assert:

			// Check that the dto object is correctly populated:
			Assert.AreEqual(SSN_GUNNA, dto.SSN);
			Assert.AreEqual(NAME_GUNNA, dto.Name);

			// Ensure that a new entity object has been created:
			var currentCount = _teacherRegistrations.Count;
			Assert.AreEqual(prevCount + 1, currentCount);

			// Get access to the entity object and assert that
			// the properties have been set:
			var newEntity = _teacherRegistrations.Last();
			Assert.AreEqual(COURSEID_VEFT_20163, newEntity.CourseInstanceID);
			Assert.AreEqual(SSN_GUNNA, newEntity.SSN);
			Assert.AreEqual(TeacherType.MainTeacher, newEntity.Type);

			// Ensure that the Unit Of Work object has been instructed
			// to save the new entity object:
			Assert.IsTrue(_mockUnitOfWork.GetSaveCallCount() > 0);
		}

        [TestMethod]
		[ExpectedException(typeof(AppObjectNotFoundException))]
		public void AddTeacher_InvalidCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.AssistantTeacher
			};

			// Act:
			_service.AddTeacherToCourse(INVALID_COURSEID, model);

			// Assert:
			// should throw exception
		}

		/// <summary>
		/// Ensure it is not possible to add a person as a teacher
		/// when that person is not registered in the system.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof (AppObjectNotFoundException))]
		public void AddTeacher_InvalidTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = INVALID_SSN,
				Type = TeacherType.MainTeacher
			};

			// Act:
			_service.AddTeacherToCourse(COURSEID_VEFT_20163, model);

			// Assert:
			// Should throw exception
		}

		/// <summary>
		/// In this test, we test that it is not possible to
		/// add another main teacher to a course, if one is already
		/// defined.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof (AppValidationException))]
		public void AddTeacher_AlreadyWithMainTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};

			// Act:
			// there already is a main teacher in this course
			_service.AddTeacherToCourse(COURSEID_VEFT_20153, model);

			// Assert:
			// Should throw exception
		}

		/// <summary>
		/// In this test, we ensure that a person cannot be added as a
		/// teacher in a course, if that person is already registered
		/// as a teacher in the given course.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(AppValidationException))]
		public void AddTeacher_PersonAlreadyRegisteredAsTeacherInCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_DABS,
				Type = TeacherType.AssistantTeacher
			};

			// Act:
			// DABS is allready in this course
			_service.AddTeacherToCourse(COURSEID_VEFT_20153, model);

			// Assert:
			// Should throw exception

		}

		#endregion
	}
}
