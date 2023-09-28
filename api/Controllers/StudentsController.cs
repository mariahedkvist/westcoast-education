using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Data.Models;
using westcoast_education.api.ViewModel;
using westcoast_education.api.ViewModels;

namespace westcoast_education.api.Controllers;

[ApiController]
[Route("api/v1/students")]
public class StudentsController : ControllerBase
{
    private readonly EducationContext _context;
    public StudentsController(EducationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> ListAll()
    {
        var result = await _context.Students
        .Include(c => c.Course)
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
            Course = s.Course!.Title ?? "Studenten är inte registrerad på någon kurs"
        })
        .ToListAsync();

        return Ok(result);
    }

    [HttpGet("id/{studentid}")]
    public async Task<ActionResult> GetById(int studentId)
    {
        var result = await _context.Students
        .Include(c => c.Course)
        .Select(s => new StudentListViewModel
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            BirthDate = s.BirthDate,
            Email = s.Email,
            Phone = s.Phone,
            Address = s.Address,
            PostalCode = s.PostalCode,
            City = s.City,
            Country = s.Country,
            Course = s.Course!.Title ?? "Studenten är inte anmäld på någon kurs"
        })
        .SingleOrDefaultAsync(s => s.Id == studentId);
        if (result is null) return BadRequest($"Läraren med ID {studentId} kunde inte hittas");
        return Ok(result);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult> GetByEmail(string email)
    {
        var result = await _context.Students
        .Include(c => c.Course)
        .Select(s => new StudentListViewModel
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            Phone = s.Phone,
            Address = s.Address,
            PostalCode = s.PostalCode,
            City = s.City,
            Country = s.Country,
            Course = s.Course!.Title ?? "Studenten är inte anmäld på någon kurs"
        })
        .SingleOrDefaultAsync(s => s.Email == email);
        if (result is null) return BadRequest($"Studenten med e-post {email} kunde inte hittas");
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> AddStudent(StudentPostViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Information saknas");

        var exists = await _context.Students.SingleOrDefaultAsync(s => s.Email!.ToUpper().Trim() == model.Email!.ToUpper().Trim());

        if (exists is not null) return BadRequest($"Student med e-post {model.Email} finns redan i systemet");

        var student = new Student
        {
            BirthDate = model.BirthDate,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            PostalCode = model.PostalCode,
            City = model.City,
            Country = model.Country,
        };

        await _context.AddAsync(student);
        if (await _context.SaveChangesAsync() > 0)
        {
            return Created(nameof(GetById), new { id = student.Id });
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPut("update/{studentid}")]
    public async Task<ActionResult> Update(int studentId, StudentUpdateViewModel model)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student is null) return BadRequest($"Lärare med ID {studentId} kunde inte hittas");

        student.BirthDate = model.BirthDate;
        student.FirstName = model.FirstName;
        student.LastName = model.LastName;
        student.Email = model.Email;
        student.Phone = model.Phone;
        student.Address = model.Address;
        student.PostalCode = model.PostalCode;
        student.City = model.City;
        student.Country = model.Country;

        _context.Students.Update(student);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPatch("enroll/{studentId}/{courseId}")]
    public async Task<ActionResult> Enroll(int studentId, int courseId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student is null) return NotFound($"Student med ID {studentId} kunde inte hittas");

        var course = await _context.Courses.FindAsync(courseId);
        if (course is null) return NotFound($"Kursen med ID {courseId} kunde inte hittas");
        student.CourseId = course.CourseId;

        _context.Students.Update(student);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }

        return StatusCode(500, "Internal Server Error");
    }

    [HttpPatch("withdraw/{studentId}")]
    public async Task<ActionResult> Withdraw(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student is null) return NotFound($"Student med ID {studentId} kunde inte hittas");

        var course = await _context.Courses.FindAsync(student.CourseId);
        if (course is null) return NotFound("Studenten är inte anmäld på någon kurs");

        course.Students!.Remove(student);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }
}