using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc_app.ViewModels.Course;
using mvc_app.ViewModels.Teacher;

namespace mvc_app.Controllers;

[Route("teachers/admin")]
public class TeachersAdminController : Controller
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClient;
    private readonly string? _baseUrl;
    private readonly JsonSerializerOptions _options;

    public TeachersAdminController(IConfiguration config, IHttpClientFactory httpClient)
    {
        _config = config;
        _httpClient = httpClient;
        _baseUrl = _config.GetSection("apiSettings:baseUrl").Value;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/teachers");

        if (!response.IsSuccessStatusCode) return Content("oops");
        var json = await response.Content.ReadAsStringAsync();
        var teachers = JsonSerializer.Deserialize<IList<TeacherListViewModel>>(json, _options);

        return View("Index", teachers);
    }

    [HttpGet("details/{teacherId}")]
    public async Task<IActionResult> Details(int teacherId)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/teachers/id/{teacherId}");
        if (!response.IsSuccessStatusCode) return Content("oops");

        var json = await response.Content.ReadAsStringAsync();
        var teacher = JsonSerializer.Deserialize<TeacherListViewModel>(json, _options);

        return View("Details", teacher);
    }
}