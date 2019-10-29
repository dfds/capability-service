using System;
using System.Linq;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models;
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
            
        }
        
        [Fact]
        public void domain_event_is_not_raised_when_Context_with_same_name_is_added_multiple_times()
        {
            // Arrange
            var contextName = "foo";
            
            var sut = new CapabilityBuilder().Build();
            
            
            // Act
            sut.AddContext(contextName);
            sut.AddContext(contextName);
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