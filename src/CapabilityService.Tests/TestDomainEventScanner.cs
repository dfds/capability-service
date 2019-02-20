using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using Xunit;

namespace DFDS.CapabilityService.Tests
{
    public class TestDomainEventScanner
    {
        [Fact]
        public void returns_expected_domain_event_implementations_from_list_of_types_that_does_not_contain_domain_events()
        {
            var sut = new DomainEventScannerBuilder().Build();
            var result = sut.GetDomainEventsFrom(Enumerable.Empty<Type>());

            Assert.Empty(result);
        }

        [Fact]
        public void returns_expected_domain_event_implementations_from_list_of_types_that_only_contain_domain_events()
        {
            var expected = new[]
            {
                typeof(FooDomainEvent),
                typeof(BarDomainEvent),
            };

            var sut = new DomainEventScannerBuilder().Build();
            var result = sut.GetDomainEventsFrom(expected);

            Assert.Equal(
                expected: expected,
                actual: result
            );
        }

        [Fact]
        public void returns_expected_domain_event_implementations_from_list_of_types_that_contains_a_mix_of_types()
        {
            var sut = new DomainEventScannerBuilder().Build();

            var result = sut.GetDomainEventsFrom(
                types: new[]
                {
                    typeof(object),
                    typeof(FooDomainEvent),
                    typeof(BarDomainEvent),
                    typeof(string)
                }
            );

            Assert.Equal(
                expected: new[]
                {
                    typeof(FooDomainEvent),
                    typeof(BarDomainEvent),
                },
                actual: result
            );
        }

        [Fact]
        public void excludes_expected_types_from_list_of_domain_event_implementations()
        {
            var sut = new DomainEventScannerBuilder().Build();

            var result = sut.GetDomainEventsFrom(
                types: new[]
                {
                    typeof(IDomainEvent),
                    typeof(FooDomainEvent),
                }
            );

            Assert.Equal(
                expected: new[] {typeof(FooDomainEvent)},
                actual: result
            );
        }

        [Fact]
        public void ensure_no_exception_is_thrown_when_all_domain_events_have_been_registered()
        {
            var sut = new DomainEventScannerBuilder()
                      .WithDomainEventRegistrationManager(new StubDomainEventRegistrationManager(isRegisteredResult: true))
                      .Build();

            var stubEventTypes = new[] {typeof(FooDomainEvent), typeof(BarDomainEvent)};

            var thrownException = Record.Exception(() => sut.EnsureNoUnregisteredDomainEventsIn(stubEventTypes));

            Assert.Null(thrownException);
        }

        [Fact]
        public void throws_expected_exception_when_domain_events_have_NOT_been_registered()
        {
            var sut = new DomainEventScannerBuilder()
                      .WithDomainEventRegistrationManager(new StubDomainEventRegistrationManager(isRegisteredResult: false))
                      .Build();

            var stubEventTypes = new[] {typeof(FooDomainEvent), typeof(BarDomainEvent)};

            Assert.Throws<MessagingException>(() => sut.EnsureNoUnregisteredDomainEventsIn(stubEventTypes));
        }

        #region helper classes

        private class FooDomainEvent : IDomainEvent { }
        private class BarDomainEvent : IDomainEvent { }

        #endregion
    }

    public class StubDomainEventRegistrationManager : IDomainEventRegistrationManager
    {
        private readonly bool _stubbedIsRegistered;

        public StubDomainEventRegistrationManager(IEnumerable<Type> domainEventTypes = null, bool isRegisteredResult = false) 
            : this(ConvertToRegistrations(domainEventTypes ?? Enumerable.Empty<Type>()), isRegisteredResult)
        {
            
        }

        public StubDomainEventRegistrationManager(IEnumerable<DomainEventRegistration> stubbedRegistrations = null, bool stubbedIsRegistered = false)
        {
            Registrations = stubbedRegistrations ?? Enumerable.Empty<DomainEventRegistration>();
            _stubbedIsRegistered = stubbedIsRegistered;
        }

        private static IEnumerable<DomainEventRegistration> ConvertToRegistrations(IEnumerable<Type> domainEvenTypes)
        {
            return domainEvenTypes.Select(x => new DomainEventRegistrationBuilder()
                                               .WithEventInstanceType(x)
                                               .Build()
            );
        }

        public IEnumerable<DomainEventRegistration> Registrations { get; }

        public IDomainEventRegistrationManager Register<TEvent>(string eventTypeName, string topicName) where TEvent : IDomainEvent
        {
            return this;
        }

        public bool IsRegistered(Type eventInstanceType)
        {
            return _stubbedIsRegistered;
        }
    }

    public class DomainEventScannerBuilder
    {
        private IDomainEventRegistrationManager _domainEventRegistrationManager;

        public DomainEventScannerBuilder()
        {
            _domainEventRegistrationManager = Dummy.Of<IDomainEventRegistrationManager>();
        }

        public DomainEventScannerBuilder WithDomainEventRegistrationManager(IDomainEventRegistrationManager domainEventRegistrationManager)
        {
            _domainEventRegistrationManager = domainEventRegistrationManager;
            return this;
        }

        public DomainEventScanner Build()
        {
            return new DomainEventScanner(_domainEventRegistrationManager);
        }
    }
}