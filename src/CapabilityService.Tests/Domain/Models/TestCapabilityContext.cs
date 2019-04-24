using System;
using System.Linq;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;


namespace DFDS.CapabilityService.Tests.Domain.Models
{
    public class TestCapabilityContext
    {
        [Fact]
        public void expected_nothing_happened_when_adding_a_existing_context()
        {
            // Arrange
            var contextName = "foo";
            var existingContext = new Context(Guid.Empty, contextName);

            var sut = new CapabilityBuilder()
                .WithContexts(existingContext)
                .Build();
            
            
            // Act
            sut.AddContext(contextName);

            
            // Assert
            Assert.Empty(sut.DomainEvents);
            Assert.Equal(existingContext, sut.Contexts.Single());
        }

        [Fact]
        public void expected_domain_event_is_raised_when_Context_is_added()
        {
            // Arrange
            var contextName = "foo";
            
            var sut = new CapabilityBuilder().Build();
            
            
            // Act
            sut.AddContext(contextName);
            
            
            // Assert
            var @event = sut.DomainEvents.Single() as ContextAddedToCapability;
            Assert.Equal(
                sut.Id,
                @event.CapabilityId
            );
            
            Assert.Equal(
                contextName,
                @event.ContextName
            );
            
            Assert.NotNull(@event.ContextId);
        }
        
        [Fact]
        public void expected_context_is_added_to_contexts()
        {
            // Arrange
            var contextName = "foo";
            
            var sut = new CapabilityBuilder().Build();
            
            
            // Act
            sut.AddContext(contextName);
            
            
            // Assert
            Assert.Contains(sut.Contexts, context => context.Name == contextName);
        }
    }
}