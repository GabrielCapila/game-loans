using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Auth.Services;
public interface ITokenService
{
    string GenerateToken(int userId, string username);
}