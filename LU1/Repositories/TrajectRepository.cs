using System.Data;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;

namespace LU1.Repositories;

public class TrajectRepository(string connectionString)
{

    public async Task<IEnumerable<Traject>> GetAll()
    {
        using (IDbConnection db = new SqlConnection(connectionString))
        {
            return await db.QueryAsync<Traject>("SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, type FROM Traject");
        }
    }
    
}