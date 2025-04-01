using System.Data;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;

namespace LU1.Repositories;

public class NoteRepository(string connectionString)
{
    public async Task<IEnumerable<Note>> GetByUserId(string userId)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            return await db.QueryAsync<Note>("SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, CAST(UserId AS UNIQUEIDENTIFIER) AS UserId, Date, Title, Content FROM Note WHERE UserId = @UserId", new { UserId = userId });
        }
    }
    
    public async Task Add(Note note)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "INSERT INTO Note (Id, UserId, Date, Title, Content) VALUES (@Id, @UserId, @Date, @Title, @Content)";
            await db.ExecuteAsync(sql, note);
        }
    }
    
    public async Task Update(Note note)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "UPDATE Note SET UserId = @UserId, Date = @Date, Title = @Title, Content = @Content WHERE Id = @Id";
            await db.ExecuteAsync(sql, note);
        }
    }
    
    public async Task Delete(string id)
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            var sql = "DELETE FROM Note WHERE Id = @Id";
            await db.ExecuteAsync(sql, new { Id = id });
        }
    }
}