using API.Extentions;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/v1/portfolio")]
[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IStockRepository _stockRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    
    public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, 
        IPortfolioRepository portfolioRepository)
    {
        _userManager = userManager;
        _stockRepository = stockRepository;
        _portfolioRepository = portfolioRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPortfolio()
    {
        var username = User.GetUserName();
        var appUser = await _userManager.FindByNameAsync(username);
        var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
        return Ok(userPortfolio);
    }

    [HttpPost]
    public async Task<IActionResult> AddPortfolio(string symbol)
    {
        var username = User.GetUserName();
        var appUser = await _userManager.FindByNameAsync(username);
        var stock = await _stockRepository.GetBySymbolAsync(symbol);

        if (stock is null)
            return BadRequest("Stock not found!");

        var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

        if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower()))
            return BadRequest("Cannot add same stock to portfolio");

        var portfolioModel = new Portfolio
        {
            StockId = stock.Id,
            AppUserId = appUser.Id
        };

        await _portfolioRepository.CreatePortfolio(portfolioModel);

        if (portfolioModel is null)
            return StatusCode(500, "Could not create");
        else
            return Created();
    }
}