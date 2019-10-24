using System;
using System.Linq;
using System.Text.RegularExpressions;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Domain.Factories
{
    public class CapabilityFactory : ICapabilityFactory
    {
        private static readonly Regex ValidNameRegex = new Regex("^[A-Z][a-zA-Z0-9\\-]{2,254}$", RegexOptions.Compiled);

        public Capability Create(string name, string description)
        {
            if (!ValidNameRegex.Match(name).Success)
            {
                throw new CapabilityValidationException(
                    "Name must be a string of length 3 to 255. consisting of only alphanumeric ASCII characters, starting with a capital letter. Underscores and hyphens are allowed.");
            }

            var id = Guid.NewGuid();
            var capability = new Capability(
                id: id,
                name: name,
                rootId: GenerateRootId(name, id),
                description: description,
                memberships: Enumerable.Empty<Membership>(),
                contexts: Enumerable.Empty<Context>()
            );

            return capability;
        }

        private static readonly string ROOTID_SALT = "fvvjaaqpagbb";

        private static string GenerateRootId(string name, Guid id)
        {
            const int maxPreservedNameLength = 22;
            
            if (name.Length < 2)
                throw new ArgumentException("Value is too short", nameof(name));

            var microHash = new HashidsNet.Hashids(ROOTID_SALT, 5, "abcdefghijklmnopqrstuvwxyz").EncodeHex(id.ToString("N")).Substring(0,5);
            
            var rootId = (name.Length > maxPreservedNameLength)
                ? $"{name.Substring(0, maxPreservedNameLength)}-{microHash}"
                : $"{name}-{microHash}";
            return rootId.ToLowerInvariant();
        }

    }
}
