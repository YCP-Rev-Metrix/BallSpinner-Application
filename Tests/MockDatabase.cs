using RevMetrix.BallSpinner.BackEnd;
using RevMetrix.BallSpinner.BackEnd.Common.POCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevMetrix.BallSpinner.Tests;
internal class MockDatabase : IDatabase
{
    public Task<Token?> LoginUser(string username, string password)
    {
        return null;
    }

    public Task<Token?> RegisterUser(string firstname, string lastname, string username, string password, string email, string phonenumber)
    {
        return null;
    }
}
