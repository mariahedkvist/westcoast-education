using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc_app.Models;
using mvc_app.ViewModels.Course;
using mvc_app.ViewModels.Student;
using static System.Net.Mime.MediaTypeNames;

namespace mvc_app.Controllers;


[Route("courses/admin")]
public class CoursesAdminController : Controller
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IConfiguration _config;
    private readonly string? _baseUrl;
    private readonly JsonSerializerOptions _options;

    public CoursesAdminController(IConfiguration config, IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        _config = config;
        _baseUrl = _config.GetSection("apiSettings:baseUrl").Value;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<IActionResult> Index()
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/courses");

        if (!response.IsSuccessStatusCode) return Content("oops");
        var json = await response.Content.ReadAsStringAsync();
        var courses = JsonSerializer.Deserialize<IList<CourseListViewModel>>(json, _options);

        return View("Index", courses);
    }

    [HttpGet("enrolled/{courseId}")]
    public async Task<IActionResult> ListEnrolledStudents(int courseId)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/courses/enrolled/{courseId}");
        if (!response.IsSuccessStatusCode) return Content("oops");

        var json = await response.Content.ReadAsStringAsync();
        var students = JsonSerializer.Deserialize<IList<StudentListViewModel>>(json, _options);
        return View("EnrolledStudents", students);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var teachersList = new List<SelectListItem>();

        using var client = _httpClient.CreateClient();

        var response = await client.GetAsync($"{_baseUrl}/teachers");
        if (!response.IsSuccessStatusCode) return Content("oops");
        var json = await response.Content.ReadAsStringAsync();
        var teachers = JsonSerializer.Deserialize<List<TeacherSettings>>(json, _options);

        foreach (var teacher in teachers)
        {
            teachersList.Add(new SelectListItem { Value = teacher.Id.ToString(), Text = teacher.FirstName + teacher.LastName });
        }

        var course = new CoursePostViewModel();

        course.Teachers = teachersList;

        return View("Create", course);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CoursePostViewModel course)
    {
        if (!ModelState.IsValid) return View("Create", course);

        var model = new
        {
            Title = course.Title,
            CourseNumber = course.CourseNumber,
            WeeksDuration = course.WeeksDuration,
            StartDate = course.StartDate,
            EndDate = course.StartDate.AddDays(course.WeeksDuration * 7),
            TeacherId = course.TeacherId
        };

        using var client = _httpClient.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, Application.Json); // gör om objektet till json

        var response = await client.PostAsync($"{_baseUrl}/courses", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        return Content("Hoppsan Kerstin");
    }

    [HttpGet("addteacher/{courseId}")]
    public async Task<IActionResult> AddTeacher(int courseId)
    {
        var teachersList = new List<SelectListItem>();

        using var client = _httpClient.CreateClient();

        var response = await client.GetAsync($"{_baseUrl}/teachers");
        if (!response.IsSuccessStatusCode) return Content("oops");
        var json = await response.Content.ReadAsStringAsync();
        var teachers = JsonSerializer.Deserialize<List<TeacherSettings>>(json, _options);

        foreach (var teacher in teachers)
        {
            teachersList.Add(new SelectListItem { Value = teacher.Id.ToString(), Text = teacher.FirstName + teacher.LastName });

        }
        var course = new CourseAddTeacherViewModel();
        course.Teachers = teachersList;
        return View("AddTeacher", course);
    }

    [HttpPatch("addteacher/{courseId}")]
    public async Task<IActionResult> AddTeacher(int courseId, CourseAddTeacherViewModel addteacher)
    {
        var model = new
        {
            TeacherId = addteacher.TeacherId
        };
        using var client = _httpClient.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, Application.Json); // gör om objektet till json
        var response = await client.PatchAsync($"{_baseUrl}/courses/addteacher", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        return Content("Hoppsan Kerstin");
    }
}