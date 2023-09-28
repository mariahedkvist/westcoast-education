using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc_app.ViewModels.Student;

namespace mvc_app.Controllers;

[Route("students/admin")]
public class StudentsAdminController : Controller
{
    private readonly IConfiguration _config;
    private readonly string? _baseUrl;
    private readonly JsonSerializerOptions _options;
    private readonly IHttpClientFactory _httpClient;
    public StudentsAdminController(IConfiguration config, IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        _config = config;
        _baseUrl = _config.GetSection("apiSettings:baseUrl").Value;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<IActionResult> Index()
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/students");

        if (!response.IsSuccessStatusCode) return Content("ooopsis");

        var json = await response.Content.ReadAsStringAsync();
        var students = JsonSerializer.Deserialize<IList<StudentListViewModel>>(json, _options);
        return View("Index", students);
    }

    [HttpGet("details/{studentId}")]
    public async Task<IActionResult> Details(int studentId)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/students/id/{studentId}");
        if (!response.IsSuccessStatusCode) return Content("oops");

        var json = await response.Content.ReadAsStringAsync();
        var student = JsonSerializer.Deserialize<StudentListViewModel>(json, _options);

        return View("Details", student);
    }
}