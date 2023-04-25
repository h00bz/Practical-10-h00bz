using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

using SMS.Web.Models;
using SMS.Data.Services;

namespace SMS.Web.Controllers;
   
public class TicketController : BaseController
{
    private readonly IStudentService svc;
    
    public TicketController()
    {
        svc = new StudentServiceDb();
    }

    // GET /ticket/index
    public IActionResult Index()
    {
        var tickets = svc.GetOpenTickets();
        return View(tickets);
    }
   
    // POST /ticket/close/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Close(int id)
    {
        // close ticket via service then check that ticket was closed
        // if not display a warning/error alert otherwise a success alert
        var t = svc.CloseTicket(id);
        if (t == null)
        {
            Alert("No such ticket found", AlertType.warning);            
        }

        Alert($"Ticket {id } closed", AlertType.info);  
        return RedirectToAction(nameof(Index));
    }       
    
    // GET /ticket/create
    public IActionResult Create()
    {
        // get list of students using service
        var students = svc.GetStudents();
                   
        var tvm = new TicketViewModel {
            // populate select list property using list of students
            Students = new SelectList(students,"Id","Name") 
        };

        // render blank form passing view model as a a parameter
        return View(tvm);
    }
   
    // POST /ticket/create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("StudentId, Issue")] TicketViewModel tvm)
    {
        // check if modelstate is valid and create ticket, display success alert and redirect to index
        if (ModelState.IsValid)
        {
            var ticket = svc.CreateTicket(tvm.StudentId, tvm.Issue);
            if (ticket != null) {
                Alert($"Ticket Created", AlertType.info);  
            } else {
                Alert($"Problem creating Ticket", AlertType.warning);
            }
            return RedirectToAction(nameof(Index));
        }

        // where needed re-populate select list property using list of students
        //var students = svc.GetStudents();
        //tvm.Students = new SelectList(students,"Id","Name"); 
  
        // redisplay the form for editing
        return View(tvm);
    }
}

