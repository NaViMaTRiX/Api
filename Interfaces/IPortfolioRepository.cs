using API.Models;

namespace API.Interfaces;

public interface IPortfolioRepository
{
    Task<List<Stock>> GetUserPortfolio(AppUser appUser);
    Task<Portfolio> CreatePortfolio(Portfolio portfolio);
}