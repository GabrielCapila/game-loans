using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Auth.Models;
public sealed class LoginResponse
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
}