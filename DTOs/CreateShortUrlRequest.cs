namespace UrlShortener.DTOs;

public class CreateShortUrlRequest
{
    
    public string Url {get ; set; }  = string.Empty;

    public string? CustomCode {get ; set ; } 
}