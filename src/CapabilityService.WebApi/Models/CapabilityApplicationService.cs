using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.CapabilityService.WebApi.Models
{
    public class CapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly ICapabilityRepository _capabilityRepository;
        private readonly IRoleService _roleService;
        private readonly Regex _nameValidationRegex = new Regex("^[A-Z][a-zA-Z0-9_\\-]{2,30}$", RegexOptions.Compiled);

        public CapabilityApplicationService(ICapabilityRepository capabilityRepository, IRoleService roleService)
        {
            _capabilityRepository = capabilityRepository;
            _roleService = roleService;
        }

        public Task<Capability> GetCapability(Guid id) => _capabilityRepository.Get(id);
        public Task<IEnumerable<Capability>> GetAllCapabilities() => _capabilityRepository.GetAll();

        public async Task<Capability> CreateCapability(string name)
        {
            if (!_nameValidationRegex.Match(name).Success) {
                throw new CapabilityValidationException("Name must be a string of length 3 to 32. consisting of only alphanumeric ASCII characters, starting with a capital letter. Underscores and hyphens are allowed.");
            }
            var capability = Capability.Create(name);
            await _capabilityRepository.Add(capability);
            await _roleService.CreateRoleFor(capability);

            return capability;
        }

        public async Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            if (capability == null)
            {
                throw new CapabilityDoesNotExistException();
            }

            capability.StartMembershipFor(memberEmail);
        }

        public async Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            if (capability == null)
            {
                throw new CapabilityDoesNotExistException();
            }

            capability.StopMembershipFor(memberEmail);
        }
    }

    public class CapabilityTransactionalDecorator : ICapabilityApplicationService
    {
        private readonly ICapabilityApplicationService _inner;
        private readonly CapabilityServiceDbContext _dbContext;

        public CapabilityTransactionalDecorator(ICapabilityApplicationService inner, CapabilityServiceDbContext dbContext)
        {
            _inner = inner;
            _dbContext = dbContext;
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities() => _inner.GetAllCapabilities();
        public Task<Capability> GetCapability(Guid id) => _inner.GetCapability(id);

        public async Task<Capability> CreateCapability(string name)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var capability = await _inner.CreateCapability(name);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return capability;
            }
        }

        public async Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.JoinCapability(capabilityId, memberEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        public async Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.LeaveCapability(capabilityId, memberEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }
    }

    public interface IRoleService
    {
        Task CreateRoleFor(Capability capability);
    }

    public class RoleService : IRoleService
    {
        private readonly IAMRoleServiceFacade _iamRoleService;
        private readonly RoleMapperServiceFacade _roleMapperService;

        public RoleService(IAMRoleServiceFacade iamRoleService, RoleMapperServiceFacade roleMapperService)
        {
            _iamRoleService = iamRoleService;
            _roleMapperService = roleMapperService;
        }
        
        public async Task CreateRoleFor(Capability capability)
        {
            var roleIdentifier = await _iamRoleService.CreateRole(capability.Name);
            await _roleMapperService.CreateRoleMapping(capability.Name, roleIdentifier);
        }
    }

    public class IAMRoleServiceFacade
    {
        private readonly HttpClient _client;

        public IAMRoleServiceFacade(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<string> CreateRole(string capabilityName)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            
            var requestPayload = new StringContent(
                content: JsonConvert.SerializeObject(new {Name = capabilityName}, serializerSettings),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );
            
            var response = await _client.PostAsync("/api/roles", requestPayload);
            response.EnsureSuccessStatusCode();

            var responsePayload = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RoleInformation>(responsePayload);

            return result.RoleArn;
        }

        private class RoleInformation
        {
            public string RoleArn { get; set; }
        }
    }

    public class RoleMapperServiceFacade
    {
        private readonly HttpClient _client;

        public RoleMapperServiceFacade(HttpClient client)
        {
            _client = client;
        }

        public async Task CreateRoleMapping(string capabilityName, string roleIdentifier)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var payload = new StringContent(
                content: JsonConvert.SerializeObject(new {RoleName = capabilityName, RoleArn = roleIdentifier}, serializerSettings),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );
            
            var response = await _client.PostAsync("/api/roles", payload);
            response.EnsureSuccessStatusCode();
        }
    }

    public class NotMemberOfCapabilityException : Exception
    {
        
    }

    public class CapabilityDoesNotExistException : Exception
    {
        
    }
}