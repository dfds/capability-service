using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class Retention
	{
		private const int ForeverDesignator = -1;
		private const Int64 DayInMs = 24 * 60 * 60 * 1000;

		public static readonly Retention Default = new Retention(7 * DayInMs);
		public static readonly Retention Forever = new Retention(ForeverDesignator);
		
		private readonly Int64 _retentionInMs;

		private Retention(Int64 retentionInMs)
		{
			_retentionInMs = retentionInMs;
		}

		public static Retention Parse(long retentionInMs)
		{
			if (retentionInMs == ForeverDesignator)
			{
				return Forever;
			}
			if (retentionInMs < ForeverDesignator)
			{
				throw new FormatException("Negative retention is not allowed (except -1 which signifies forever)");
			}
			if (retentionInMs == 0)
			{
				throw new FormatException("A retention of zero (0) is not allowed");
			}

			return new Retention(retentionInMs);
		}

		public static implicit operator string(Retention retention)
		{
			if (retention == Forever)
			{
				return "-1";
			}
			return $"{retention._retentionInMs}ms";
		}
	}
}
