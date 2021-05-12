using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Api
{
	[Authorize(AuthenticationSchemes = "AzureADBearer")]
	[ApiController]
	[Route("api/v1/kafka/cluster")]
	public class ClusterController : ControllerBase
	{
		private readonly IClusterRepository _clusterRepository;

		public ClusterController(IClusterRepository clusterRepository)
		{
			_clusterRepository = clusterRepository;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll()
		{
			var clusters = await _clusterRepository.GetAllAsync();

			return Ok(clusters);
		}
	}
}
