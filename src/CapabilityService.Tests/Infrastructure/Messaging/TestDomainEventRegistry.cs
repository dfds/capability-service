using System;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.Helpers;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Messaging
{
    public class TestDomainEventRegistry
    {
        [Fact]
        public void returns_expected_registrations_when_initialized()
        {
            var sut = new DomainEventRegistryBuilder().Build();
            Assert.Empty(sut.Registrations);
        }

        [Fact]
        public void returns_expected_registrations_when_registering_single_event()
        {
            var stubRegistration = new DomainEventRegistrationBuilder()
                           .WithEventInstanceType<FooDomainEvent>()
                           .Build();

            var sut = new DomainEventRegistryBuilder().Build();

            sut.Register<FooDomainEvent>(stubRegistration.EventType, stubRegistration.Topic);

            Assert.Equal(
                    expected: new[] {stubRegistration},
                    actual: sut.Registrations,
                    comparer: new PropertiesComparer<DomainEventRegistration>()
                );
        }

        [Fact]
        public void returns_expected_registrations_when_registering_multiple_events()
        {
            var stubRegistration1 = new DomainEventRegistrationBuilder()
                           .WithEventInstanceType<FooDomainEvent>()
                           .WithEventType("foo-event-type")
                           .WithTopic("foo-topic")
                           .Build();

            var stubRegistration2 = new DomainEventRegistrationBuilder()
                           .WithEventInstanceType<BarDomainEvent>()
                           .WithEventType("bar-event-type")
                           .WithTopic("bar-topic")
                           .Build();

            var sut = new DomainEventRegistryBuilder().Build();

            sut.Register<FooDomainEvent>(stubRegistration1.EventType, stubRegistration1.Topic);
            sut.Register<BarDomainEvent>(stubRegistration2.EventType, stubRegistration2.Topic);

            Assert.Equal(
                expected: new[] {stubRegistration1, stubRegistration2},
                actual: sut.Registrations,
                comparer: new PropertiesComparer<DomainEventRegistration>()
            );
        }

        [Fact]
        public void returns_expected_topic()
        {
            var stubRegistration = new DomainEventRegistrationBuilder()
                                   .WithEventInstanceType<FooDomainEvent>()
                                   .Build();

            var sut = new DomainEventRegistryBuilder().Build();
            sut.Register<FooDomainEvent>(stubRegistration.EventType, stubRegistration.Topic);

            var result = sut.GetTopicFor(stubRegistration.EventType);

            Assert.Equal(
                expected: stubRegistration.Topic,
                actual: result
            );
        }

        [Fact]
        public void throws_expected_exception_when_no_topic_is_registered_for_an_event_type()
        {
            var stubEventTypeName = "foo";
            var sut = new DomainEventRegistryBuilder().Build();

            Assert.Throws<MessagingException>(() => sut.GetTopicFor(stubEventTypeName));
        }

        [Fact]
        public void returns_expected_event_type_name()
        {
            var stubEvent = new FooDomainEvent();

            var stubRegistration = new DomainEventRegistrationBuilder()
                                   .WithEventInstanceType(stubEvent)
                                   .Build();

            var sut = new DomainEventRegistryBuilder().Build();
            sut.Register<FooDomainEvent>(stubRegistration.EventType, stubRegistration.Topic);

            var result = sut.GetTypeNameFor(stubEvent);

            Assert.Equal(
                expected: stubRegistration.EventType,
                actual: result
            );
        }

        [Fact]
        public void throws_expected_exception_when_no_event_type_name_is_registered_for_an_event_instance()
        {
            var stubEvent = new FooDomainEvent();
            var sut = new DomainEventRegistryBuilder().Build();

            Assert.Throws<MessagingException>(() => sut.GetTypeNameFor(stubEvent));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("NewUnknownEvent")]
        public void throws_expected_exception_when_no_instance_type_is_registered(string eventName)
        {
            var stubEvent = new FooDomainEvent();
            var stubRegistration = new DomainEventRegistrationBuilder()
                .WithEventInstanceType(stubEvent)
                .Build();
            
            var sut = new DomainEventRegistryBuilder().Build();
            sut.Register<FooDomainEvent>(stubRegistration.EventType, stubRegistration.Topic);

            Assert.Throws<MessagingHandlerNotAvailable>(() => sut.GetInstanceTypeFor(eventName));
        }

        [Fact]
        public void returns_expected_instance_type()
        {
            var stubEvent = new FooDomainEvent();
            var stubRegistration = new DomainEventRegistrationBuilder()
                .WithEventInstanceType(stubEvent)
                .Build();
            
            var sut = new DomainEventRegistryBuilder().Build();
            sut.Register<FooDomainEvent>(stubRegistration.EventType, stubRegistration.Topic);

            var result = sut.GetInstanceTypeFor(stubRegistration.EventType);
            
            Assert.Equal(typeof(FooDomainEvent), result);
        }
        
        

        #region helper classes

        private class FooDomainEvent : IDomainEvent { }
        private class BarDomainEvent : IDomainEvent { }

        #endregion
    }
}