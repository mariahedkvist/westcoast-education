using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Data.Models;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModels;

namespace westcoast_education.api.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CoursesController : ControllerBase
{
    private readonly EducationContext _context;
    public CoursesController(EducationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> ListAll()
    {
        var result = await _context.Courses
        .Include(t => t.Teacher)
        .Select(c => new CourseListViewModel
        {
            CourseId = c.CourseId,
            Title = c.Title,
            CourseNumber = c.CourseNumber,
            Teacher = c.Teacher != null ? c.Teacher!.FirstName + " " + c.Teacher.LastName : "Kursen har ingen tilldelad lärare",
            WeeksDuration = c.WeeksDuration,
            StartDate = c.StartDate,
            EndDate = c.EndDate
        })
        .ToListAsync();

        return Ok(result);
    }

    [HttpGet("enrolled/{courseId}")]
    public async Task<ActionResult> ListEnrolledStudents(int courseId)
    {
        var result = await _context.Students
        .Include(c => c.Course)
        .Where(c => c.Course!.CourseId == courseId)
        .Select(s => new StudentListViewModel
        {
            Id = s.Id,
            BirthDate = s.BirthDate,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            Phone = s.Phone,
            Address = s.Address,
            PostalCode = s.PostalCode,
            City = s.City,
            Country = s.Country,
            Course = s.Course!.Title
        })
        .ToListAsync();

        return Ok(result);
    }

    [HttpGet("id/{courseId}")]
    public async Task<ActionResult> GetById(int courseId)
    {
        var result = await _context.Courses
        .Select(c => new CourseListViewModel
        {
            CourseId = c.CourseId,
            Title = c.Title,
            CourseNumber = c.CourseNumber,
            Teacher = c.Teacher!.FirstName + c.Teacher.LastName ?? "Kursen har ingen tilldelad lärare",
            WeeksDuration = c.WeeksDuration,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
        })
        .SingleOrDefaultAsync(c => c.CourseId == courseId);
        if (result is null) return BadRequest($"Kursen med ID {courseId} kunde inte hittas");
        return Ok(result);
    }

    [HttpGet("coursenum/{coursenum}")]
    public async Task<ActionResult> GetByCourseNum(int courseNum)
    {
        var result = await _context.Courses
        .Select(c => new CourseListViewModel
        {
            CourseId = c.CourseId,
            Title = c.Title,
            CourseNumber = c.CourseNumber,
            Teacher = c.Teacher!.FirstName + c.Teacher.LastName ?? "Kursen har ingen tilldelad lärare",
            WeeksDuration = c.WeeksDuration,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
        })
        .SingleOrDefaultAsync(c => c.CourseNumber == courseNum);
        return Ok(result);
    }

    [HttpGet("title/{title}")]
    public async Task<ActionResult> GetByTitle(string title)
    {
        var result = await _context.Courses
        .Where(c => c.Title!.ToLower().Replace(" ", "") == title.ToLower().Replace(" ", ""))
        .Select(c => new CourseListViewModel
        {
            CourseId = c.CourseId,
            Title = c.Title,
            CourseNumber = c.CourseNumber,
            Teacher = c.Teacher!.FirstName + c.Teacher.LastName ?? "Kursen har ingen tilldelad lärare",
            WeeksDuration = c.WeeksDuration,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
        })
        .ToListAsync();

        if (result is null || !result.Any()) return NotFound($"Inga kurser med namnet {title} kunde hittas i systemet");

        return Ok(result);
    }

    [HttpGet("startdate/{startDate}")]
    public async Task<ActionResult> GetByStartDate(string startDate)
    {
        var result = await _context.Courses
        .Where(c => c.StartDate.ToString() == startDate)
        .Select(c => new CourseListViewModel
        {
            CourseId = c.CourseId,
            Title = c.Title,
            CourseNumber = c.CourseNumber,
            Teacher = c.Teacher!.FirstName + c.Teacher.LastName ?? "Kursen har ingen tilldelad lärare",
            WeeksDuration = c.WeeksDuration,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
        })
        .ToListAsync();
        if (result is null || !result.Any()) return NotFound($"Inga kurser med startdatumet {startDate} kunde hittas i systemet");
        return Ok(result);
    }

    [HttpPut("update/{courseId}")]
    public async Task<ActionResult> Update(int courseId, CourseUpdateViewModel model)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course is null) return BadRequest($"Kursen med ID {courseId} kunde inte hittas");

        course.Title = model.Title;
        course.CourseNumber = model.CourseNumber;
        course.WeeksDuration = model.WeeksDuration;
        course.StartDate = model.StartDate;
        course.EndDate = model.StartDate.AddDays(model.WeeksDuration * 7);

        _context.Courses.Update(course);

        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");

    }

    [HttpPost]
    public async Task<ActionResult> AddCourse(CoursePostViewModel model)
    {
        var exists = await _context.Courses.SingleOrDefaultAsync(
            c => c.CourseNumber == model.CourseNumber &&
            c.StartDate == model.StartDate
        );

        if (exists is not null) return BadRequest($"Kurs med nummer {model.CourseNumber} som startar {model.StartDate} finns redan i systemet");

        var course = new Course
        {
            Title = model.Title,
            CourseNumber = model.CourseNumber,
            WeeksDuration = model.WeeksDuration,
            StartDate = model.StartDate,
            EndDate = model.StartDate.AddDays(model.WeeksDuration * 7),
            TeacherId = model.TeacherId
        };

        await _context.Courses.AddAsync(course);

        if (await _context.SaveChangesAsync() > 0)
        {
            return Created(nameof(GetById), new { id = course.CourseId });
        }
        return StatusCode(500, "Internal Server Error");

    }

    [HttpPatch("addteacher/{courseId}")]
    public async Task<ActionResult> AddTeacher(int courseId, CourseAddTeacherViewModel model)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course is null) return NotFound($"Kursen med ID {courseId} kunde inte hittas");

        course.TeacherId = model.TeacherId;

        _context.Courses.Update(course);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPatch("removeteacher/{courseId}")]
    public async Task<ActionResult> RemoveTeacher(int courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course is null) return NotFound($"Kursen med ID {courseId} finns inte i systemet");

        var teacher = await _context.Teachers.FindAsync(course.TeacherId);
        if (teacher is null) return NotFound($"Kursen har ingen undervisande lärare");

        teacher.Courses!.Remove(course);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPatch("full/{courseId}")]
    public async Task<ActionResult> MarkAsFull(int courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course is null) return NotFound($"Kursen med ID {courseId} finns inte i systemet");

        course.Status = CourseStatusEnum.Full;

        _context.Courses.Update(course);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPatch("finished/{courseId}")]
    public async Task<ActionResult> MarkAsFinished(int courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course is null) return NotFound($"Kursen med ID {courseId} finns inte i systemet");

        course.Status = CourseStatusEnum.Finished;
        _context.Courses.Update(course);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");

    }

    [HttpDelete("{courseId}")]
    public async Task<ActionResult> Delete(int courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course is null) return NotFound($"Kursen med ID {courseId} kunde inte hittas");

        _context.Courses.Remove(course);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

}