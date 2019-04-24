using System;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
    public class TestContext
    {
        [Fact]
        public void Id_And_Name_Is_Set_By_Constructor()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "foo";
            
            
            // Act
            var context = new WebApi.Domain.Models.Context(id, name);
            
            
            // Assert
            Assert.Equal(id, context.Id);
            Assert.Equal(name, context.Name);
        }
    }
}