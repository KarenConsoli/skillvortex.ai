using Microsoft.AspNetCore.Mvc;
using SkillVortex.Application.Services;
using SkillVortex.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillVortex.ApiService.Controllers
{
    public class ApplicationController : BaseController
    {
        private readonly ApplicationService _applicationService;

        public ApplicationController(ApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Entities.Application>> GetApplication(Guid id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
                return NotFound();

            return Ok(application);
        }

        [HttpGet("talent/{talentId}")]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Application>>> GetApplicationsByTalent(Guid talentId)
        {
            var applications = await _applicationService.GetApplicationsByTalentIdAsync(talentId);
            return Ok(applications);
        }

        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Application>>> GetApplicationsByJob(Guid jobId)
        {
            var applications = await _applicationService.GetApplicationsByJobIdAsync(jobId);
            return Ok(applications);
        }

        [HttpPost]
        public async Task<ActionResult> ApplyToJob([FromBody] ApplicationRequest request)
        {
            try
            {
                var applicationId = await _applicationService.ApplyToJobAsync(request.TalentId, request.JobId);
                return CreatedAtAction(nameof(GetApplication), new { id = applicationId }, new { id = applicationId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateApplicationStatus(Guid id, [FromBody] ApplicationStatusUpdateRequest request)
        {
            var result = await _applicationService.UpdateApplicationStatusAsync(id, request.Status);
            if (!result)
                return BadRequest(new { message = "Invalid status transition" });

            return Ok(new { message = $"Application status updated to {request.Status}" });
        }
    }

    public class ApplicationRequest
    {
        public Guid TalentId { get; set; }
        public Guid JobId { get; set; }
    }

    public class ApplicationStatusUpdateRequest
    {
        public ApplicationStatus Status { get; set; }
    }
}