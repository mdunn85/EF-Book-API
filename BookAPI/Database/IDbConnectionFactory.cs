using System.Data;

namespace BookAPI.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
