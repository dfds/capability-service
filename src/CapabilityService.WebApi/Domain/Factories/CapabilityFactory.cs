using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Domain.Factories
{
    public class CapabilityFactory : ICapabilityFactory
    {

        public ValueTask<Capability> Create(string name, string description)
        {
	        var capabilityName = new CapabilityName(name);
            var id = Guid.NewGuid();
            var capability = new Capability(
                id: id,
                name: capabilityName,
                rootId: GenerateRootId(name, id),
                description: description,
                memberships: Enumerable.Empty<Membership>(),
                contexts: Enumerable.Empty<Context>()
            );

            return new ValueTask<Capability>(capability);
        }

        private const string ROOTID_SALT = "fvvjaaqpagbb";

        private static string GenerateRootId(string name, Guid id)
        {
            const int maxPreservedNameLength = 22;

            if (name.Length < 2)
                throw new ArgumentException("Value is too short", nameof(name));

            var microHash = new HashidsNet.Hashids(ROOTID_SALT, 5, "abcdefghijklmnopqrstuvwxyz")
                .EncodeHex(id.ToString("N")).Substring(0, 5);

            var rootId = (name.Length > maxPreservedNameLength)
                ? $"{name.Substring(0, maxPreservedNameLength)}-{microHash}"
                : $"{name}-{microHash}";
            return rootId.ToLowerInvariant();
        }
    }
}
