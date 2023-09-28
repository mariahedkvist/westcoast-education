using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Data.Models;
using westcoast_education.api.ViewModels;

namespace westcoast_education.api.Controllers;

[ApiController]
[Route("api/v1/teachers")]
public class TeachersController : ControllerBase
{
    private readonly EducationContext _context;
    public TeachersController(EducationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> ListAll()
    {
        var result = await _context.Teachers
        .Select(t => new TeacherListViewModel
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            BirthDate = t.BirthDate,
            Email = t.Email,
            Phone = t.Phone,
            Address = t.Address,
            PostalCode = t.PostalCode,
            City = t.City,
            Country = t.Country,
            Skills = t.Skills!.Select(
                s => new SkillListViewModel
                {
                    Name = s.Name
                }
            ).
            // obs! Select() skickar tillbaka en IEnumerable, därför blir det konverteringsknas utan ToList()
            ToList(),
        })
        .ToListAsync();
        return Ok(result);
    }

    [HttpGet("teaching/{teacherid}")]
    public async Task<ActionResult> ListCourses(int teacherId)
    {
        var result = await _context.Courses
        .Include(t => t.Teacher)
        .Where(t => t.Teacher!.Id == teacherId)
        .Select(c => new CourseListViewModel
        {
            CourseId = c.CourseId,
            Title = c.Title,
            CourseNumber = c.CourseNumber,
            Teacher = c.Teacher!.FirstName + " " + c.Teacher.LastName,
            WeeksDuration = c.WeeksDuration,
            StartDate = c.StartDate,
            EndDate = c.EndDate
        })
        .ToListAsync();

        return Ok(result);
    }

    [HttpGet("id/{teacherId}")]
    public async Task<ActionResult> GetById(int teacherId)
    {
        var result = await _context.Teachers
        .Select(t => new TeacherListViewModel
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            BirthDate = t.BirthDate,
            Email = t.Email,
            Phone = t.Phone,
            Address = t.Address,
            PostalCode = t.PostalCode,
            City = t.City,
            Country = t.Country,
            Skills = t.Skills!.Select(
                s => new SkillListViewModel
                {
                    Name = s.Name
                }
            ).ToList(),
            Courses = t.Courses!.Select(
                c => new CourseListViewModel{
                    Title = c.Title
                }
            ).ToList()
        })
        .SingleOrDefaultAsync(t => t.Id == teacherId);
        if (result is null) return BadRequest($"Läraren med ID {teacherId} kunde inte hittas");
        return Ok(result);
    }
    [HttpGet("email/{email}")]
    public async Task<ActionResult> GetByEmail(string email)
    {
        var result = await _context.Teachers
        .Select(t => new TeacherListViewModel
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            Phone = t.Phone,
            Address = t.Address,
            PostalCode = t.PostalCode,
            City = t.City,
            Country = t.Country,
            Skills = t.Skills!.Select(
                s => new SkillListViewModel
                {
                    Name = s.Name
                }
            ).ToList()
        })
        .SingleOrDefaultAsync(t => t.Email == email);
        if (result is null) return BadRequest($"Läraren med e-post {email} kunde inte hittas");
        return Ok(result);
    }

    [HttpPut("update/{teacherid}")]
    public async Task<ActionResult> Update(int teacherId, TeacherUpdateViewModel model)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher is null) return BadRequest($"Lärare med ID {teacherId} kunde inte hittas");

        teacher.BirthDate = model.BirthDate;
        teacher.FirstName = model.FirstName;
        teacher.LastName = model.LastName;
        teacher.Email = model.Email;
        teacher.Phone = model.Phone;
        teacher.Address = model.Address;
        teacher.PostalCode = model.PostalCode;
        teacher.City = model.City;
        teacher.Country = model.Country;

        _context.Teachers.Update(teacher);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPost]
    public async Task<ActionResult> Create(TeacherPostViewModel model)
    {
        var exists = await _context.Teachers.SingleOrDefaultAsync(
            t => t.Email == model.Email);
        if (exists is not null) return BadRequest($"Läraren med e-post {model.Email} finns redan i systemet");

        var teacher = new Teacher
        {
            BirthDate = model.BirthDate,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            PostalCode = model.PostalCode,
            City = model.City,
            Country = model.Country
        };
        await _context.Teachers.AddAsync(teacher);
        if (await _context.SaveChangesAsync() > 0)
        {
            return Created(nameof(GetById), new { id = teacher.Id });
        }
        return StatusCode(500, "Internal Server Error");
    }


    [HttpPost("skill")]
    public async Task<ActionResult> CreateSkill(SkillPostViewModel model)
    {
        var exists = await _context.Skills.SingleOrDefaultAsync(
            s => s.Name!.Trim().ToLower() == model.Name!.Trim().ToLower());

        if (exists is not null) return BadRequest($"Kompetensen {model.Name} finns redan i systemet");

        var skill = new Skill
        {
            Name = model.Name
        };

        await _context.Skills.AddAsync(skill);
        if (await _context.SaveChangesAsync() > 0)
        {
            return Created(nameof(GetById), new { id = skill.Id });
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPatch("skill/{teacherId}/{skillId}")]
    public async Task<ActionResult> AddSkill(int teacherId, int skillId)
    {
        var skill = await _context.Skills.SingleOrDefaultAsync(s => s.Id == skillId);
        if (skill is null) return NotFound($"Kompetens med ID {skillId} kunde inte hittas");

        var teacher = await _context.Teachers.SingleOrDefaultAsync(t => t.Id == teacherId);
        if (teacher is null) return NotFound($"Lärare med ID {teacherId} kunde inte hittas");

        if (teacher.Skills is null) teacher.Skills = new List<Skill>();
        teacher.Skills.Add(skill);

        _context.Teachers.Update(teacher);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

}