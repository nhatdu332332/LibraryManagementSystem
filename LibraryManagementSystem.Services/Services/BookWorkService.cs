using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Repositories.Interfaces;
using System.Collections.Generic;

public class BookWorkService
{

    private readonly IBookWorkRepository repo;

    public BookWorkService(IBookWorkRepository repository)
    {
        repo = repository;
    }
    public List<BookWork> GetAll() => repo.GetAll();

    public List<BookWork> Search(string key) => repo.Search(key);

    public void Add(BookWork b) => repo.Add(b);

    // New async method to add a BookWork together with multiple categories/authors
    public async Task<bool> AddBookWorkAsync(BookWork book, System.Collections.Generic.List<int> categoryIds, System.Collections.Generic.List<int> authorIds)
    {
        return await repo.AddBookWorkAsync(book, categoryIds, authorIds);
    }

    public void Update(BookWork b) => repo.Update(b);

    public void Delete(int id) => repo.Delete(id);
    public BookWork GetById(int id)
    {
        return repo.GetById(id);
    }

    public async Task<BookWork?> GetBookWorkDetailAsync(int workId)
    {
        return await repo.GetBookWorkDetailAsync(workId);
    }
}