using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.DTOs;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Services;
using Npgsql;


namespace UrlShortener.Controller;

[ApiController]
[Route("api/urls")]
public class UrlController : ControllerBase
{
    private readonly AppDbContext _db; 
    private readonly UrlShortenerService _service; 

    public UrlController(
        AppDbContext db ,
        UrlShortenerService service)
    {
        _db = db;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Shorten(CreateShortUrlRequest request)
    {
       
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("Url is required ");
        }
        
        if(!Uri.TryCreate(request.Url , UriKind.Absolute , out var uri))
        {
            return BadRequest("Invalid URL");
        }



        if (!string.IsNullOrWhiteSpace(request.CustomCode))
        {
            if(request.CustomCode.Length < 3  ||  request.CustomCode.Length  >  20)
            {
                return BadRequest("Custom Code must be between 3 to 20 characters");
            }
            if(!System.Text.RegularExpressions.Regex.IsMatch(request.CustomCode  , @"^[a-zA-Z0-9-]+$"))
            {
                return BadRequest(
            "Custom code can contain only letters, numbers and '-'.");
            }
        }

        string shortCode ;

        if (!string.IsNullOrWhiteSpace(request.CustomCode))
        {
            shortCode = request.CustomCode;
        }
        else {
        // do
        // {
            shortCode = _service.GenerateShortCode();
        // }
        // while(await _db.shortUrls.AnyAsync(x => x.ShortCode == shortCode));
        }
        var shortUrls = new ShortUrl
        {
            Id = Guid.NewGuid(),
            ShortCode = shortCode,
            OriginalUrl = uri.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _db.shortUrls.Add(shortUrls);
        
        try{
        await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException &&  postgresException.SqlState == "23505")
        {
            return Conflict("Short Code Already Exists");
        }
        var response  = new CreateShortUrlResponse{
            ShortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}"
        };

        return Ok(response);

    }
}