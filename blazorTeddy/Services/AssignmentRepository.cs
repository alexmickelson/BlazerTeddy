using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{

    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly Func<IDbConnection> getDbConnection;
        private readonly ILogger<AssignmentRepository> logger;

        public AssignmentRepository(Func<IDbConnection> getDbConnection,
                                    ILogger<AssignmentRepository> logger)
        {
            this.getDbConnection = getDbConnection;
            this.logger = logger;
        }

        public async Task<Assignment> GetAssignmentAsync(int assignmentId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryFirstAsync<Assignment>(
                    @"select * from Assignment
                      where AssignmentId = @assignmentId;",
                    new { assignmentId }
                );
            }
        }

        public async Task AddAssignmentAsync(Assignment assignment)
        {
            using (var dbConnection = getDbConnection())
            {
                assignment.AssignmentId = await dbConnection.QueryFirstAsync<int>(
                    @"insert into Assignment (AssignmentName, AssignmentDescription, CourseId)
                      values (@AssignmentName, @AssignmentDescription, @CourseId)
                      returning AssignmentId;",
                    assignment
                );
            }
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsAsync(int courseId)
        {
            using (var dbConnection = getDbConnection())
            {
                return await dbConnection.QueryAsync<Assignment>(
                    @"select * from Assignment
                      where CourseId = @courseId;",
                    new { courseId }
                );
            }
        }

        public async Task UpdateAssignmentAsync(Assignment assignment)
        {
            using (var dbConnection = getDbConnection())
            {
                await dbConnection.ExecuteAsync(
                    @"update Assignment set 
                        AssignmentName = @AssignmentName,
                        AssignmentDescription = @AssignmentDescription,
                        CourseId = @CourseId
                    where AssignmentId = @AssignmentId",
                    assignment
                );
            }
        }
    }
}