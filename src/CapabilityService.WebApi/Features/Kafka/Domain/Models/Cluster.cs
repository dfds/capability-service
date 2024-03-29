﻿using System;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class Cluster : AggregateRoot<Guid>
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Enabled { get; set; }
		public string ClusterId { get; set; }

		private Cluster(Guid id, string clusterId, string name, string description, bool enabled)
		{
			Id = id;
			Name = name;
			Description = description;
			ClusterId = clusterId;
			Enabled = enabled;
		}

		public static Cluster Create(string clusterId, string name, string description, bool enabled, Guid id)
		{
			Guid objId;
			if (id.Equals(Guid.Empty))
			{
				objId = Guid.NewGuid();
			}
			else
			{
				objId = id;
			}
			
			// Potential spot to add logic for restrictions, e.g. max length of Description. See the Topic class for an example.
			var cluster = new Cluster(objId, clusterId, name, description, enabled);
			
			cluster.RaiseEvent(new ClusterCreated(cluster));

			return cluster;
		}

		public static Cluster FromDao(Infrastructure.Persistence.EntityFramework.DAOs.Cluster clusterDao)
		{
			return new Cluster(clusterDao.Id, clusterDao.ClusterId, clusterDao.Name, clusterDao.Description, clusterDao.Enabled);
		}
	}
}
