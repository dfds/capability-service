using System;
using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
	public class ValueObjectTestSubject : ValueObject
	{
		public string Value1 { get; }
		public int Value2 { get; }
		public Guid Value3 { get; }

		public ValueObjectTestSubject(string value1, int value2, Guid value3)
		{
			Value1 = value1;
			Value2 = value2;
			Value3 = value3;
		}
		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Value1;
			yield return Value2;
			yield return Value3;
		}
	}
}
