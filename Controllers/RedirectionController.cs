using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;


namespace UrlShortener.Controller;

[ApiController]
public class RedirectController : ControllerBase
{
    private readonly AppDbContext _db ; 

    public RedirectController(AppDbContext db)
    {
        _db = db ;
    }

    [HttpGet("{shortcode}")]
    public async Task<IActionResult> RedirectUrl(string shortcode)
    {
        var shortUrls = await  _db.shortUrls.FirstOrDefaultAsync(x => x.ShortCode == shortcode);
        // Console.WriteLine("Get into redirect api");
        if(shortUrls == null)
        {
            return NotFound("Short Url Not Found!");
        }
        await _db.shortUrls.Where(x => x.ShortCode == shortcode).ExecuteUpdateAsync(s=> s.SetProperty(p => p.ClickCount , p=> p.ClickCount+1));
        return Redirect(shortUrls.OriginalUrl);

    }
}
