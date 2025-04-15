using System.Data;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;

namespace LU1.Repositories;

public interface ITrajectRepository
{
    Task<IEnumerable<Traject>> GetAll();
}

public class TrajectRepository(string connectionString) : ITrajectRepository
{

    public async Task<IEnumerable<Traject>> GetAll()
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            return await db.QueryAsync<Traject>("SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, type FROM Traject");
        }
    }
    
}