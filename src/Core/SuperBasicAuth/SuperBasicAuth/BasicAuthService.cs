using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devmonster.Core.SuperBasicAuth;

public interface IBasicAuthService
{
    public ValueTask<bool> Authenticate(string username, string password);
}
public class BasicAuthService : IBasicAuthService
{

    readonly IOptions<SuperBasicAuthConfig> _config;

    public BasicAuthService(IOptions<SuperBasicAuthConfig> config)
    {
        _config = config;
    }


    public async ValueTask<bool> Authenticate(string username, string password)
    {
        string[] credentials = _config.Value.BasicCredentials.Split(":", 2);

        return await Task.FromResult(string.Equals(username, credentials[0]) &&
            string.Equals(password, credentials[1]));

    }
}