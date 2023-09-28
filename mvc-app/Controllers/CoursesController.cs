using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc_app.ViewModels.Course;

namespace mvc_app.Controllers;

[Route("courses")]
public class CoursesController : Controller
{
    private readonly IConfiguration _config;
    private readonly string? _baseUrl;
    private readonly JsonSerializerOptions _options;
    private readonly IHttpClientFactory _httpClient;
    public CoursesController(IConfiguration config, IHttpClientFactory httpClient)
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

        if(!response.IsSuccessStatusCode) return Content("oops");
        var json = await response.Content.ReadAsStringAsync();
        var courses = JsonSerializer.Deserialize<IList<CourseListViewModel>>(json, _options);

        return View("Index", courses);
    }
}