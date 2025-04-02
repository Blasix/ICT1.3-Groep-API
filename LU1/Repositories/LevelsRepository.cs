using System.Data;
using Dapper;
using LU1.Models;
using Microsoft.Data.SqlClient;

namespace LU1.Repositories
{
    public class LevelsRepository(string connectionString)
    {
        public async Task<IEnumerable<Level>> GetLevelsByStepAndTrajectId(int step, string trajectId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var convertedTrajectId = Guid.Parse(trajectId);

                var sql = "SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, " +
                          "CAST(TrajectId AS UNIQUEIDENTIFIER) AS TrajectId, " +
                          "Step, Url, Tekst, TotalSteps " +
                          "FROM Levels " +
                          "WHERE Step = @Step AND TrajectId = @TrajectId";

                return await db.QueryAsync<Level>(sql, new { Step = step, TrajectId = convertedTrajectId });
            }
        }
    }
}
