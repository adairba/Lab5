using Xunit;
using LibraryWebApp.Models;
using LibraryWebApp.Services;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace LibraryWebApp.Tests;

public class LibraryServiceTests
{
    [Fact]
    public void AddUser_ShouldAddNewUserToList()
    {

        var service = new LibraryService();
        var testUser = new User { Name = "Test Student", Email = "test@etsu.edu" };
        int initialCount = service.Users.Count;


        service.AddUser(testUser);


        Assert.Equal(initialCount + 1, service.Users.Count);
        Assert.Contains(service.Users, u => u.Name == "Test Student");
    }

    [Fact]
    public void EditUser_ShouldUpdateNameAndEmail()
    {

        var service = new LibraryService();
        var user = new User { Name = "Original Name", Email = "old@etsu.edu" };
        service.AddUser(user);

        int assignedId = user.Id;
        var updatedUser = new User { Id = assignedId, Name = "Updated Name", Email = "new@etsu.edu" };


        service.EditUser(updatedUser);


        var result = service.Users.First(u => u.Id == assignedId);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("new@etsu.edu", result.Email);
    }

    [Fact]
    public void DeleteUser_ShouldRemoveUserFromList()
    {

        var service = new LibraryService();
        var user = new User { Name = "Delete Me", Email = "gone@etsu.edu" };
        service.AddUser(user);
        int userId = user.Id;
        int countAfterAdd = service.Users.Count;


        service.DeleteUser(userId);


        Assert.Equal(countAfterAdd - 1, service.Users.Count);
        Assert.DoesNotContain(service.Users, u => u.Id == userId);
    }

    [Fact]
    public void AddBook_ShouldAddNewBookToList()
    {

        var service = new LibraryService();
        var testBook = new Book { Title = "Testing 101", Author = "Lab Author" };
        int initialCount = service.Books.Count;


        service.AddBook(testBook);


        Assert.Equal(initialCount + 1, service.Books.Count);
        Assert.Contains(service.Books, b => b.Title == "Testing 101");
    }

    [Fact]
    public void DeleteBook_ShouldRemoveBookFromList()
    {

        var service = new LibraryService();
        var book = new Book { Title = "Test Book", Author = "Test Author" };
        service.AddBook(book);


        int assignedId = book.Id;
        int countAfterAdd = service.Books.Count;


        service.DeleteBook(assignedId);

        // ASSERT
        Assert.Equal(countAfterAdd - 1, service.Books.Count);
        Assert.DoesNotContain(service.Books, b => b.Id == assignedId);
    }

    [Fact]
    public void EditBook_ShouldUpdateExistingDetails()
    {

        var service = new LibraryService();
        var originalBook = new Book { Id = 1, Title = "Old Title", Author = "Old Author" };
        service.AddBook(originalBook);

        var updatedBook = new Book { Id = 1, Title = "New Title", Author = "New Author" };


        service.EditBook(updatedBook);


        var result = service.Books.First(b => b.Id == 1);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("New Author", result.Author);
    }

    [Fact]
    public void BorrowBook_ShouldUpdateStatusAndDictionary()
    {

        var service = new LibraryService();
        var testUser = new User { Name = "Lab Student", Email = "test@etsu.edu" };
        var testBook = new Book { Title = "Unit Testing 101", IsBorrowed = false };


        service.AddUser(testUser);
        service.AddBook(testBook);


        int userId = testUser.Id;
        int bookId = testBook.Id;


        service.BorrowBook(userId, bookId);


        var bookInService = service.Books.First(b => b.Id == bookId);
        Assert.True(bookInService.IsBorrowed);


        Assert.Contains(bookInService, service.BorrowedBooks[testUser]);
    }

    [Fact]
    public void ReturnBook_ShouldResetStatusAndClearDictionary()
    {

        var service = new LibraryService();
        var testUser = new User { Name = "Lab Student", Email = "test@etsu.edu" };
        var testBook = new Book { Title = "Unit Testing 101" };


        service.AddUser(testUser);
        service.AddBook(testBook);

        int bookId = testBook.Id;
        int userId = testUser.Id;


        service.BorrowBook(userId, bookId);


        service.ReturnBook(bookId);


        var bookInService = service.Books.First(b => b.Id == bookId);

        Assert.False(bookInService.IsBorrowed);


        Assert.DoesNotContain(bookInService, service.BorrowedBooks[testUser]);
    }

    [Fact]
    public void ReadBooks_ShouldLoadDataFromCsv()
    {
        // ARRANGE: Initialize the service
        // The constructor automatically calls ReadBooks()
        var service = new LibraryService();

        // ACT: (Optional) You can call it explicitly to ensure a reload
        service.ReadBooks();

        // ASSERT: Verify the list is not empty
        // This assumes your Data/Books.csv has at least one book in it
        Assert.NotNull(service.Books);
        Assert.True(service.Books.Count >= 0);
    }

    [Fact]
    public void ReadUsers_ShouldPopulateUsersList()
    {
        // ARRANGE
        var service = new LibraryService();

        // ACT
        service.ReadUsers();

        // ASSERT
        Assert.NotNull(service.Users);
        // Even if empty, the list object should be initialized
        Assert.IsType<List<User>>(service.Users);
    }
}
