using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.Tests.TestDoubles;
using DFDS.TeamService.WebApi.Models;
using Xunit;

namespace DFDS.TeamService.Tests
{
    public class TestTeamApplicationService
    {
        [Theory]
        [InlineData("an-otherwise-acceptable-name")]
        [InlineData("AName!")]
        [InlineData("Aa")]
        [InlineData("A0123456789012345678901234567891")]
        public async Task cannot_create_teams_with_invalid_names(string input) {
            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository())
                .Build();

            await Assert.ThrowsAsync<TeamValidationException>(() => sut.CreateTeam(input));
        }

        [Theory]
        [InlineData("AName")]
        [InlineData("AZ0")]
        [InlineData("A012345678901234567890123456789")]
        public async Task can_create_team_with_an_acceptable_name(string input) {
            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository())
                .Build();

            await sut.CreateTeam(input);
        }

        [Fact]
        public async Task expected_member_is_added_to_team()
        {
            var team = new TeamBuilder().Build();

            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository(team))
                .Build();

            var stubMemberEmail = "foo@bar.com";
            
            await sut.JoinTeam(team.Id, stubMemberEmail);
            
            Assert.Equal(new[]{stubMemberEmail}, team.Members.Select(x => x.Email));
        }

        [Fact]
        public async Task throws_exception_when_adding_member_to_a_non_existing_team()
        {
            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository())
                .Build();

            var nonExistingTeamId = Guid.Empty;
            var dummyMemberEmail = "foo@bar.com";

            await Assert.ThrowsAsync<TeamDoesNotExistException>(() => sut.JoinTeam(nonExistingTeamId, dummyMemberEmail));
        }

        [Fact]
        public async Task adding_the_same_member_to_a_team_multiple_times_yields_single_membership()
        {
            var team = new TeamBuilder().Build();

            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository(team))
                .Build();

            var stubMemberEmail = "foo@bar.com";
            
            await sut.JoinTeam(team.Id, stubMemberEmail);
            await sut.JoinTeam(team.Id, stubMemberEmail);
            
            Assert.Equal(new[]{stubMemberEmail}, team.Members.Select(x => x.Email));
        }

        [Fact]
        public async Task expected_member_is_removed_from_team()
        {
            var stubMemberEmail = "foo@bar.com";

            var team = new TeamBuilder()
                .WithMembers(stubMemberEmail)
                .Build();

            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository(team))
                .Build();

            await sut.LeaveTeam(team.Id, stubMemberEmail);

            Assert.Empty(team.Members);
        }

        [Fact]
        public async Task removing_non_existing_member_from_a_team_throws_exception()
        {
            var team = new TeamBuilder().Build();

            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository(team))
                .Build();

            var nonExistingMemberEmail = "foo@bar.com";

            await Assert.ThrowsAsync<NotMemberOfTeamException>(() => sut.LeaveTeam(team.Id, nonExistingMemberEmail));
        }

        [Fact]
        public async Task removing_member_from_non_existing_team_throws_exception()
        {
            var sut = new TeamApplicationServiceBuilder()
                .WithTeamRepository(new StubTeamRepository())
                .Build();

            var nonExistingTeamId = Guid.Empty;
            var dummyMemberId = "foo@bar.com";

            await Assert.ThrowsAsync<TeamDoesNotExistException>(() => sut.LeaveTeam(nonExistingTeamId, dummyMemberId));
        }
    }

    public class TeamApplicationServiceBuilder
    {
        private ITeamRepository _teamRepository;
        private IRoleService _roleService;

        public TeamApplicationServiceBuilder()
        {
            _teamRepository = Dummy.Of<ITeamRepository>();
            _roleService = Dummy.Of<IRoleService>();
        }

        public TeamApplicationServiceBuilder WithTeamRepository(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
            return this;
        }

        public TeamApplicationServiceBuilder WithRoleService(IRoleService roleService)
        {
            _roleService = roleService;
            return this;
        }
        
        public TeamApplicationService Build()
        {
            return new TeamApplicationService(
                    teamRepository: _teamRepository,
                    roleService: _roleService
                );
        }
    }
}