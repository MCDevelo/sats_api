using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
}
