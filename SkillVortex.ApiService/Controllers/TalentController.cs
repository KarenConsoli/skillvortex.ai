using Microsoft.AspNetCore.Mvc;
using SkillVortex.Application.Services;
using SkillVortex.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SkillVortex.ApiService.Controllers
{
    public class TalentController : BaseController
    {
        private readonly TalentService _talentService;

        public TalentController(TalentService talentService)
        {
            _talentService = talentService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Talent>> GetTalent(Guid id)
        {
            var talent = await _talentService.GetTalentByIdAsync(id);
            if (talent == null)
                return NotFound();

            return Ok(talent);
        }

        [HttpGet("verified")]
        public async Task<ActionResult> GetVerifiedTalents()
        {
            var talents = await _talentService.GetVerifiedTalentsAsync();
            return Ok(talents);
        }

        [HttpPost("{id}/verification")]
        public async Task<ActionResult> SetVerificationStatus(Guid id, [FromBody] VerificationRequest request)
        {
            var result = await _talentService.SetIAVerificationStatusAsync(id, request.IsVerified);
            if (!result)
                return NotFound();

            return Ok(new { message = request.IsVerified ? "Talent verified successfully" : "Talent verification removed" });
        }

        [HttpPost("{id}/welcome-call")]
        public async Task<ActionResult> SetWelcomeCallStatus(Guid id, [FromBody] WelcomeCallRequest request)
        {
            var result = await _talentService.SetWelcomeCallStatusAsync(id, request.HasPassed, request.Notes);
            if (!result)
                return NotFound();

            return Ok(new { message = request.HasPassed ? "Welcome call marked as passed" : "Welcome call marked as failed" });
        }

        [HttpPost]
        public async Task<ActionResult> CreateTalent([FromBody] TalentCreationRequest request)
        {
            // Create a new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                Role = "Talent",
                CreatedAt = DateTime.UtcNow
            };

            // Create a new talent
            var talent = new Talent
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                LinkedInUrl = request.LinkedInUrl,
                GitHubUrl = request.GitHubUrl,
                PortfolioUrl = request.PortfolioUrl,
                HourlyRate = request.HourlyRate,
                CountryCode = request.CountryCode,
                EnglishLevel = 0, // Will be set during welcome call
                HasPassedWelcomeCall = false,
                IsIAVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            var talentId = await _talentService.CreateTalentAsync(talent);

            return CreatedAtAction(nameof(GetTalent), new { id = talentId }, new { id = talentId });
        }
    }

    public class VerificationRequest
    {
        public bool IsVerified { get; set; }
    }

    public class WelcomeCallRequest
    {
        public bool HasPassed { get; set; }
        public string Notes { get; set; }
    }

    public class TalentCreationRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string LinkedInUrl { get; set; }
        public string GitHubUrl { get; set; }
        public string PortfolioUrl { get; set; }
        public decimal HourlyRate { get; set; }
        public string CountryCode { get; set; }
    }
}