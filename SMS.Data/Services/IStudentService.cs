
using SMS.Data.Entities;
	
namespace SMS.Data.Services;

// This interface describes the operations that a StudentService class should implement
public interface IStudentService
{
    // Initialise the repository (database)  
    void Initialise();
    
    // ---------------- Student Management --------------
    List<Student> GetStudents();
    Student GetStudent(int id);
    Student GetStudentByEmail(string email);
    Student AddStudent(Student s);
    Student UpdateStudent(Student updated);  
    bool DeleteStudent(int id);
  

    // ---------------- Ticket Management ---------------
    Ticket CreateTicket(int studentId, string issue);
    Ticket UpdateTicket(int id, string issue);
    Ticket GetTicket(int id);
    Ticket CloseTicket(int id);
    bool   DeleteTicket(int id);
    IList<Ticket> GetOpenTickets();  

}
    
