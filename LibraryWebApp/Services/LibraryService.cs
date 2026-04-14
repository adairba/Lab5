using LibraryWebApp.Models;
using System.IO;

namespace LibraryWebApp.Services
{
    public class LibraryService : ILibraryService
    {
        public List<Book> Books { get; private set; } = new List<Book>();
        public List<User> Users { get; private set; } = new List<User>();
        public Dictionary<User, List<Book>> BorrowedBooks { get; private set; } = new Dictionary<User, List<Book>>();

        private readonly string _booksPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Books.csv");
        private readonly string _usersPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Users.csv");

        public LibraryService()
        {
            ReadBooks();
            ReadUsers();
        }

        // --- LOADING DATA ---
        public void ReadBooks()
        {
            try
            {
                Books.Clear();
                if (!File.Exists(_booksPath)) return;

                foreach (var line in File.ReadLines(_booksPath))
                {
                    var fields = line.Split(',');
                    if (fields.Length >= 4) // Keep this at 4 so it doesn't skip your data!
                    {
                        var book = new Book
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Title = fields[1].Trim(),
                            Author = fields[2].Trim(),
                            ISBN = fields[3].Trim(),
                            // Default to false if the 5th column is missing, otherwise parse it
                            IsBorrowed = fields.Length >= 5 && bool.Parse(fields[4].Trim())
                        };
                        Books.Add(book);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV: {ex.Message}");
            }
        }

        public void ReadUsers()
        {
            try
            {
                Users.Clear();
                if (!File.Exists(_usersPath)) return;
                foreach (var line in File.ReadLines(_usersPath))
                {
                    var fields = line.Split(',');
                    if (fields.Length >= 3)
                    {
                        Users.Add(new User
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Name = fields[1].Trim(),
                            Email = fields[2].Trim()
                        });
                    }
                }
            }
            catch (Exception) { /* Handle error or log */ }
        }

        // --- BOOK CRUD ---
        public void AddBook(Book book)
        {
            book.Id = Books.Any() ? Books.Max(b => b.Id) + 1 : 1;
            Books.Add(book);
            WriteBooksToCsv();
        }

        public void EditBook(Book updatedBook)
        {
            var existing = Books.FirstOrDefault(b => b.Id == updatedBook.Id);
            if (existing != null)
            {
                existing.Title = updatedBook.Title;
                existing.Author = updatedBook.Author;
                existing.ISBN = updatedBook.ISBN;
                WriteBooksToCsv();
            }
        }

        public void DeleteBook(int bookId)
        {
            var book = Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                Books.Remove(book);
                WriteBooksToCsv();
            }
        }

        // --- USER CRUD ---
        public void AddUser(User user)
        {
            user.Id = Users.Any() ? Users.Max(u => u.Id) + 1 : 1;
            Users.Add(user);
            WriteUsersToCsv();
        }

        public void EditUser(User updatedUser)
        {
            var existing = Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existing != null)
            {
                existing.Name = updatedUser.Name;
                existing.Email = updatedUser.Email;
                WriteUsersToCsv();
            }
        }

        public void DeleteUser(int userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                Users.Remove(user);
                WriteUsersToCsv();
            }
        }

        // --- BORROW & RETURN ---
        public void BorrowBook(int userId, int bookId)
        {
            // 1. Find the specific book and user in the main lists
            var user = Users.FirstOrDefault(u => u.Id == userId);
            var book = Books.FirstOrDefault(b => b.Id == bookId);

            if (user != null && book != null)
            {
                // 2. IMPORTANT: Update the status property of the book
                book.IsBorrowed = true;

                // 3. Track the relationship in your dictionary
                if (!BorrowedBooks.ContainsKey(user))
                {
                    BorrowedBooks[user] = new List<Book>();
                }

                // Add to the list if not already there
                if (!BorrowedBooks[user].Contains(book))
                {
                    BorrowedBooks[user].Add(book);
                }

                // 4. Save the change so it stays 'Borrowed' even if you refresh
                WriteBooksToCsv();
            }
        }

        public void ReturnBook(int bookId)
        {
            // 1. Find the book in the master list
            var book = Books.FirstOrDefault(b => b.Id == bookId);

            if (book != null)
            {
                // 2. Flip status back to false
                book.IsBorrowed = false;

                // 3. Remove it from the dictionary tracking (if you're using it)
                foreach (var userEntry in BorrowedBooks)
                {
                    if (userEntry.Value.Any(b => b.Id == bookId))
                    {
                        userEntry.Value.RemoveAll(b => b.Id == bookId);
                        break; // Found the user, we can stop looking
                    }
                }

                // 4. Save the "Available" status back to the CSV
                WriteBooksToCsv();
                Console.WriteLine($"Book {bookId} has been returned.");
            }
        }

        // --- FILE SAVING LOGIC ---
        private void WriteBooksToCsv()
        {
            // Note the {b.IsBorrowed} at the end!
            var lines = Books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.ISBN},{b.IsBorrowed}");

            // This writes over the old Books.csv with the new 5-column version
            File.WriteAllLines(_booksPath, lines);
        }

        private void WriteUsersToCsv()
        {
            var lines = Users.Select(u => $"{u.Id},{u.Name},{u.Email}");
            File.WriteAllLines(_usersPath, lines);
        }


    }
}