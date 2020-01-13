using System;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
	public class TestValueObject
	{
		[Fact]
		public void Equals_is_false_if_one_attribute_is_not_the_same()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.NewGuid()
			);

			var valueObjectTestSubject02 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);

			Assert.NotEqual(
				valueObjectTestSubject01,
				valueObjectTestSubject02
			);
		}

		[Fact]
		public void Equals_is_true_if_all_attributes_are_the_same()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);

			var valueObjectTestSubject02 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);

			Assert.Equal(
				valueObjectTestSubject01,
				valueObjectTestSubject02
			);
		}

		[Fact]
		public void Equals_is_false_if_one_is_null()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.NewGuid()
			);

			ValueObjectTestSubject valueObjectTestSubject02 = null;

			Assert.False(valueObjectTestSubject01.Equals(valueObjectTestSubject02));
		}

		[Fact]
		public void Equals_is_false_one_is_different_type()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.NewGuid()
			);

			string valueObjectTestSubject02 = "baa";

			Assert.False(valueObjectTestSubject01.Equals(valueObjectTestSubject02));
		}

		[Fact]
		public void Not_equal_operator_returns_false__if_both_is_null()
		{
			ValueObjectTestSubject valueObjectTestSubject01 = null;
			ValueObjectTestSubject valueObjectTestSubject02 = null;

			Assert.False(valueObjectTestSubject01 != valueObjectTestSubject02);
		}

		[Fact]
		public void Not_equal_operator_returns_true_if_one_is_null()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);

			ValueObjectTestSubject valueObjectTestSubject02 = null;

			Assert.True(valueObjectTestSubject01 != valueObjectTestSubject02);
		}

		[Fact]
		public void Not_equal_operator_returns_false_if_both_has_same_properties()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);

			var valueObjectTestSubject02 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);


			Assert.False(valueObjectTestSubject01 != valueObjectTestSubject02);
		}


		[Fact]
		public void GetHashCode_returns_same_number_if_both_has_same_properties()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);

			var valueObjectTestSubject02 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);


			Assert.Equal(
				valueObjectTestSubject01.GetHashCode(),
				valueObjectTestSubject02.GetHashCode()
			);
		}
		
		[Fact]
		public void GetHashCode_returns_different_number_if_one_property_is_different()
		{
			var valueObjectTestSubject01 = new ValueObjectTestSubject(
				"baa",
				1337,
				Guid.Empty
			);

			var valueObjectTestSubject02 = new ValueObjectTestSubject(
				"foo",
				1337,
				Guid.Empty
			);


			Assert.NotEqual(
				valueObjectTestSubject01.GetHashCode(),
				valueObjectTestSubject02.GetHashCode()
			);
		}
	}
}
