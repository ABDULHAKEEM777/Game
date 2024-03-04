using Cleverbit.CodingTask.Data;
using Cleverbit.CodingTask.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cleverbit.CodingTask.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly CodingTaskContext _context;
        public MatchController(CodingTaskContext context)
        {
            _context = context;
        }
        [HttpGet("matches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            return await _context.Matches.Include(m => m.User).ToListAsync();
        }

        [HttpPost("play")]
        public async Task<ActionResult<int>> Play([FromBody] UserMatch userMatch)
        {
            var match = await _context.Matches.FindAsync(userMatch.MatchId);

            var userMt = _context.UserMatches.FirstOrDefault(u => u.MatchId == userMatch.MatchId && u.UserId == userMatch.UserId);

            if (match == null || match.ExpiryTimestamp < DateTime.UtcNow)
            {
                return -1;
            }
            if (userMt != null)
            {
                return userMt.GeneratedNumber;
            }
            // Simulate generating a random number between 0 and 100
            userMatch.GeneratedNumber = new Random().Next(0, 101);

            _context.UserMatches.Add(userMatch);
            await _context.SaveChangesAsync();

            return userMatch.GeneratedNumber;
        }

        [HttpGet("refresh")]
        public async Task<ActionResult<IEnumerable<Match>>> RefreshResults()
        {
            var matches = _context.Matches.Where(m => m.WinnerUserId == null).ToList();
            foreach (Match match in matches)
            {
                if (match != null && match.ExpiryTimestamp < DateTime.UtcNow)
                {
                    var matchUsers = _context.UserMatches.Where(mu => mu.MatchId == match.Id).ToList();

                    if (matchUsers.Count > 0)
                    {
                        // Determine the user who generated the largest number
                        var winner = matchUsers.OrderByDescending(mu => mu.GeneratedNumber).First();

                        // Store the user ID of the winner in the MatchUser table
                        match.WinnerUserId = winner.UserId;

                        _context.Matches.Update(match);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return await _context.Matches.Include(m => m.User).ToListAsync();
        }
    }
}
