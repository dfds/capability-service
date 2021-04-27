using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services
{
	public class TopicDomainService : ITopicDomainService
	{
		private readonly ITopicRepository _topicRepository;
		private readonly IClusterRepository _clusterRepository;

		public TopicDomainService(ITopicRepository topicRepository, IClusterRepository clusterRepository)
		{
			_topicRepository = topicRepository;
			_clusterRepository = clusterRepository;
		}

		public async Task CreateTopic(Topic topic, bool dryRun)
		{
			var existingTopics = await _topicRepository.GetAllAsync();
			var existingClusters = await _clusterRepository.GetAllAsync();

			if (existingTopics.Any(t =>
			{
				return t.Name.Equals(topic.Name) && t.KafkaClusterId.Equals(topic.KafkaClusterId);
			})) 
			{ throw new TopicAlreadyExistException(topic.Name);}

			if (!existingClusters.Any(c => c.Id.Equals(topic.KafkaClusterId))) { throw new ClusterDoesNotExistException(topic.KafkaClusterId.ToString());}

			if (!existingClusters.First(c => c.Id.Equals(topic.KafkaClusterId)).Enabled) { throw new ClusterIsDisabledException();}

			if(dryRun) return;
			
			await _topicRepository.AddAsync(topic);
		}

		public async Task DeleteTopic(string name, string clusterId)
		{
			var existingTopics = await _topicRepository.GetAllAsync();
			try
			{
				var topic = existingTopics.First(t => t.Name.Name.Equals(name) && t.KafkaClusterId.ToString().Equals(clusterId));
				await _topicRepository.DeleteAsync(topic);
			}
			catch (InvalidOperationException)
			{
				throw new TopicDoesNotExistException(TopicName.FromString(name));
			}
		}

		public async Task<IEnumerable<Topic>> GetAllTopics()
		{
			return await _topicRepository.GetAllAsync();
		}

		public async Task<Cluster> GetClusterByClusterId(string clusterId)
		{
			var results = await _clusterRepository.GetAllAsync();
			try
			{
				return results.First(c => c.ClusterId.Equals(clusterId));
			}
			catch (Exception)
			{
				throw new ClusterDoesNotExistException(clusterId);
			}
		}

		public async Task<Cluster> GetClusterById(Guid id)
		{
			var results = await _clusterRepository.GetAllAsync();
			try
			{
				return results.First(c => c.Id.Equals(id));
			}
			catch (Exception)
			{
				throw new ClusterDoesNotExistException(id.ToString());
			}
		}
		
		public async Task<IEnumerable<Cluster>> GetAllClusters()
		{
			return await _clusterRepository.GetAllAsync();
		}
	}
}
