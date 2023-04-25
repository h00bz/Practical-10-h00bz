using Microsoft.EntityFrameworkCore;
using SMS.Data.Entities;
using SMS.Data.Repository;

namespace SMS.Data.Services;

public class StudentServiceDb : IStudentService
{
    private readonly DataContext db;

    public StudentServiceDb()
    {
        db = new DataContext();
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // -------- Student Related Operations ------------

    // retrieve list of Students
    public List<Student> GetStudents()
    {
        return db.Students.ToList();
    }


    // Retrive student by Id 
    public Student GetStudent(int id)
    {
        return db.Students
                 .Include(s => s.Tickets)                                
                 .FirstOrDefault(s => s.Id == id);
    }

    // Add a new student
    public Student AddStudent(Student s)
    {
        // check if student with email exists            
        var exists = GetStudentByEmail(s.Email);
        if (exists != null)
        {
            return null;
        } 
        // check grade is valid
        if (s.Grade < 0 || s.Grade > 100)
        {
            return null;
        }

        // create new student
        var student = new Student
        {
            Name = s.Name,
            Course = s.Course,
            Email = s.Email,
            Age = s.Age,
            Grade = s.Grade,
            PhotoUrl = s.PhotoUrl
        };
        db.Students.Add(student); // add student to the list
        db.SaveChanges();
        return student; // return newly added student
    }

    // Delete the student identified by Id returning true if 
    // deleted and false if not found
    public bool DeleteStudent(int id)
    {
        var s = GetStudent(id);
        if (s == null)
        {
            return false;
        }
        db.Students.Remove(s);
        db.SaveChanges();
        return true;
    }

    // Update the student with the details in updated 
    public Student UpdateStudent(Student updated)
    {
        // verify the student exists 
        var student = GetStudent(updated.Id);
        if (student == null)
        {
            return null;
        }

        // verify email is still unique
        var exists = GetStudentByEmail(updated.Email);
        if (exists != null && exists.Id != updated.Id)
        {
            return null;
        }

        // verify grade is valid
        if (updated.Grade < 0 || updated.Grade > 100)
        {
            return null;
        }

        // update the details of the student retrieved and save
        student.Name = updated.Name;
        student.Email = updated.Email;
        student.Course = updated.Course;
        student.Age = updated.Age;
        student.Grade = updated.Grade;
        student.PhotoUrl = updated.PhotoUrl;

        db.SaveChanges();
        return student;
    }

    public Student GetStudentByEmail(string email)
    {
        return db.Students.FirstOrDefault(s => s.Email == email);
    }

    // ===================== Ticket Management ==========================
    public Ticket CreateTicket(int studentId, string issue)
    {
        var student = GetStudent(studentId);
        if (student == null) return null;

        var ticket = new Ticket
        {
            // Id created by Database
            Issue = issue,        
            StudentId = studentId,
            // set by default in model but we can override here if required
            CreatedOn = DateTime.Now,
            Active = true,
        };
        db.Tickets.Add(ticket);
        db.SaveChanges(); // write to database
        return ticket;
    }

    public Ticket UpdateTicket(int id, string issue)
    {
        var ticket = GetTicket(id);
        if (ticket == null) return null;

        ticket.Issue = issue;              
                    
        db.Tickets.Update(ticket);
        db.SaveChanges(); // write to database
        return ticket;
    }

    public Ticket GetTicket(int id)
    {
        // return ticket and related student or null if not found
        return db.Tickets
                    .Include(t => t.Student)
                    .FirstOrDefault(t => t.Id == id);
    }

    public Ticket CloseTicket(int id)
    {
        var ticket = GetTicket(id);
        // if ticket does not exist or is already closed return null
        if (ticket == null || !ticket.Active) return null;
        
        // ticket exists and is active so close
        ticket.Active = false;
        
        db.SaveChanges(); // write to database
        return ticket;
    }

    public bool DeleteTicket(int id)
    {
        // find ticket
        var ticket = GetTicket(id);
        if (ticket == null) return false;
        
        // remove ticket 
        var result = db.Tickets.Remove(ticket);
        
        db.SaveChanges();
        return true;
    }

    // Retrieve all tickets and the student associated with the ticket
    public IList<Ticket> GetAllTickets()
    {
        return db.Tickets
                    .Include(t => t.Student)
                    .ToList();
    }

    // Retrieve all open tickets (Active)
    public IList<Ticket> GetOpenTickets()
    {
        // return open tickets with associated students
        return db.Tickets
                    .Include(t => t.Student) 
                    .Where(t => t.Active)
                    .ToList();
    } 

}
