using System.Security.Cryptography;
using System.Security.Principal;

namespace UrlShortener.Services;

public class UrlShortenerService
{
    private const string Characters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


    public string GenerateShortCode()
    {
        Span<char> result  = stackalloc char[6];

        for(int i =0 ; i < result.Length ; i++)
        {
            
            int index  = RandomNumberGenerator.GetInt32(Characters.Length);

            result[i] = Characters[index];

        }

        return new string(result[..3]) + "-" + new string(result[3..]);
    }
}