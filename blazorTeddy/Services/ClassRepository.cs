using System;
using System.Data;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public class ClassRepository : IClassRepository
    {
        private readonly Func<IDbConnection> getDbConnection;

        public ClassRepository(Func<IDbConnection> getDbConnection)
        {
            this.getDbConnection = getDbConnection;
        }

        public Task AddClassAsync(ClassModel classModel)
        {
            throw new NotImplementedException();
        }

        public Task<ClassModel> GetClassAsync(int classId)
        {
            throw new NotImplementedException();
        }
    }
}