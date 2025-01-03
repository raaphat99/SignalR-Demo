using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Data;
using SignalRDemo.Hubs;
using SignalRDemo.Models;

namespace SignalRDemo.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHubContext<CourseHub> _hubContext;
        public CourseController(ApplicationContext context, IHubContext<CourseHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Insert(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("AddNewCourse", course);

                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }
    }
}
