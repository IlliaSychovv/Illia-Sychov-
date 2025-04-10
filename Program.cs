using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public int UniqueCode { get; set; }

    public Book(string title, string author, int year, int uniqueCode)
    {
        Title = title;
        Author = author;
        Year = year;
        UniqueCode = uniqueCode;
    }
}

class Program
{
    static string filePath = "books.json";

    static readonly string[] Authors = {"Mykola", "Illia", "Denis"};
    static readonly object lockObj = new object();
    static readonly Random random = new Random();

    static async Task Main(string[] args)
    {
        List<Book> books = LoadBooksFromJSON();

        if (books == null)
        {
            books = new List<Book>
            {
                new Book("CLR via C#", "Джефри Рихтер", 2012, 1234),
                new Book("Программирование на С# для начинающих", "Джон Смилли", 2020, 102),
                new Book("С# 12 и .NET 8. Базовый курс", "Марк Дж. Прайс", 2023, 5665)
            };
        }

        var list = from book in books
                   select book;

        foreach (var book in list)
        {
            Console.WriteLine($"Книга: {book.Title}, Aвтор: {book.Author}, Год выпуска книги: {book.Year}, Уникальный код книги: {book.UniqueCode}.");
        }

        List<Task> tasks = new List<Task>();

        for (int i = 0; i < 100; i++)
        {
            tasks.Add(ChangeAuthorBook(books));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("\nИзмененные книги:");
        foreach (var book in books)
        {
            Console.WriteLine($"Книга: {book.Title}, Автор, после изминения: {book.Author}, Год: {book.Year}, Уникальный код: {book.UniqueCode}");
        }

        SaveBooksToJSON(books);

        Console.WriteLine("\nКакую книгу вы хотите взять?");
        string operation1 = Console.ReadLine();

        switch (operation1)
        {
            case "CLR via C#":
                Console.WriteLine("Автор данной книги: Джефри Рихтер, год выпуска: 2012, уникальный код книги: 1234.\nВы взяли эту книгу, теперь ее статус ЗАНЯТО.");
                break;
            case "Программирование на С# для начинающих":
                Console.WriteLine("Автор данной книги: Джон Смилли, год выпуска: 2020, уникальный код книги: 0102.\nВы взяли эту книгу, теперь ее статус ЗАНЯТО.");
                break;
            case "С# 12 и .NET 8. Базовый курс":
                Console.WriteLine("Автор данной книги: Марк Дж. Прайс, год выпуска: 2023, уникальный код книги: 5665.\nВы взяли эту книгу, теперь ее статус ЗАНЯТО.");
                break;
            default:
                Console.WriteLine("Такой книги в этой библиотеке нет!");
                break;
        }

        Console.WriteLine("\nКакую книгу вы хотите вернуть?");
        string operation2 = Console.ReadLine();

        switch (operation2)
        {
            case "CLR via C#":
                Console.WriteLine($"Вы вернули книгу: CLR via C#, теперь ее статус ДОСТУПНО.");
                break;
            case "Программирование на С# для начинающих":
                Console.WriteLine("Вы вернули книгу: Программирование на С# для начинающих, теперь ее статус ДОСТУПНО.");
                break;
            case "С# 12 и .NET 8. Базовый курс":
                Console.WriteLine("Вы вернули книгу: С# 12 и .NET 8. Базовый курс, теперь ее статус ДОСТУПНО.");
                break;
            default:
                Console.WriteLine("Такой книги в этой библиотеке нет!");
                break;
        }

        Console.WriteLine("\nВведите уникальный код книги, чтобы ее удалить");
        int code = Convert.ToInt32(Console.ReadLine());

        Book bookToDelete = books.Find(book => book.UniqueCode == code);

        if (bookToDelete != null)
        {
            books.Remove(bookToDelete);
            Console.WriteLine($"Книга с уникальным кодом {code} была удалена.");
        }
        else
        {
            Console.WriteLine("Нет книги с таким кодом!");
        }

        Console.WriteLine("\nОставшиеся книги в коллекции: ");
        foreach (var book in books)
        {
            Console.WriteLine($"Книга: {book.Title}, Автор: {book.Author}, Год выпуска книги: {book.Year}, Уникальный код книги: {book.UniqueCode}");
        }

        SaveBooksToJSON(books);
    }

    static async Task ChangeAuthorBook(List<Book> books)
    {
        lock (lockObj)
        {
            foreach (var book in books)
            {
                book.Author = Authors[random.Next(Authors.Length)];
                Console.WriteLine($"Автор изменен для книги '{book.Title}'");
            }
        }

        await Task.Delay(1);
    }

    static void SaveBooksToJSON(List<Book> books)
    {
        string json = JsonConvert.SerializeObject(books, Formatting.Indented);
        File.WriteAllText(filePath, json);
        Console.WriteLine("\nДанные были сохранены в JSON файл.");
    }

    static List<Book> LoadBooksFromJSON()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Book>>(json);
        }
        return null;
    }
}