using LibraryWebApp.Models;

namespace LibraryWebApp.Services
{
    public interface ILibraryService
    {
        List<Book> Books { get; }
        List<User> Users { get; }
        Dictionary<User, List<Book>> BorrowedBooks { get; }
        
        void ReadBooks();
        void ReadUsers();
        
        void AddBook(Book book);
        void EditBook(Book book);
        void DeleteBook(int bookId);
        
        void AddUser(User user);
        void EditUser(User user);
        void DeleteUser(int userId);
        
        void BorrowBook(int userId, int bookId);
        void ReturnBook(int bookId);
    }
}