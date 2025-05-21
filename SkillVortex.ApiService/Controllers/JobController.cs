using Microsoft.AspNetCore.Mvc;
using SkillVortex.Application.Services;
using SkillVortex.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillVortex.ApiService.Controllers
{
    public class JobController : BaseController
    {
        private readonly JobService _jobService;

        public JobController(JobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(Guid id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpGet("open")]
        public async Task<ActionResult<IEnumerable<Job>>> GetOpenJobs()
        {
            var jobs = await _jobService.GetOpenJobsAsync();
            return Ok(jobs);
        }

        [HttpPost]
        public async Task<ActionResult> CreateJob([FromBody] JobCreationRequest request)
        {
            // Create a new job
            var job = new Job
            {
                Title = request.Title,
                CompanyAlias = request.CompanyAlias,
                JobRaw = request.JobDescription,
                SeniorityRequired = request.SeniorityRequired,
                JobRole = request.JobRole,
                EmploymentType = "Contractor", // Default for MVP
                TotalSeats = request.TotalSeats,
                MatcherId = request.MatcherId
            };

            var jobId = await _jobService.CreateJobAsync(job);

            return CreatedAtAction(nameof(GetJob), new { id = jobId }, new { id = jobId });
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateJobStatus(Guid id, [FromBody] JobStatusUpdateRequest request)
        {
            var result = await _jobService.UpdateJobStatusAsync(id, request.Status);
            if (!result)
                return NotFound();

            return Ok(new { message = $"Job status updated to {request.Status}" });
        }
    }

    public class JobCreationRequest
    {
        public string Title { get; set; }
        public string CompanyAlias { get; set; }
        public string JobDescription { get; set; }
        public string SeniorityRequired { get; set; }
        public string JobRole { get; set; }
        public int TotalSeats { get; set; } = 1;
        public Guid MatcherId { get; set; }
    }

    public class JobStatusUpdateRequest
    {
        public string Status { get; set; }
    }
}