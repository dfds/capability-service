using System;
using System.Net;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.Tests.TestDoubles;
using DFDS.TeamService.WebApi.Features.Teams.Application;
using DFDS.TeamService.WebApi.Features.Teams.Domain;
using Xunit;

namespace DFDS.TeamService.Tests.Features.Teams
{
    public class TestTeamsRoutes
    {
        [Fact]
        public async Task get_list_of_teams_returns_expected_status_code_when_no_teams_are_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamService>(new StubTeamService())
                    .Build();

                var response = await client.GetAsync("api/teams");

                Assert.Equal(
                    expected: HttpStatusCode.OK,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task get_list_of_teams_returns_expected_status_code_when_multiple_teams_are_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeamList = new[]
                {
                    new TeamBuilder().Build(),
                    new TeamBuilder().Build()
                };

                var client = builder
                    .WithService<ITeamService>(new StubTeamService(teams: stubTeamList))
                    .Build();

                var response = await client.GetAsync("api/teams");

                Assert.Equal(
                    expected: HttpStatusCode.OK,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task get_single_team_by_id_returns_expected_status_code_when_team_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = builder
                    .WithService<ITeamService>(new StubTeamService(teams: stubTeam))
                    .Build();

                var response = await client.GetAsync($"api/teams/{Guid.NewGuid()}");

                Assert.Equal(
                    expected: HttpStatusCode.OK,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task get_single_team_by_id_returns_expected_status_code_when_team_is_NOT_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamService>(new StubTeamService())
                    .Build();

                var response = await client.GetAsync($"api/teams/{Guid.NewGuid()}");

                Assert.Equal(
                    expected: HttpStatusCode.NotFound,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task post_returns_expected_status_code_when_creating_a_team()
        {
            using (var clientBuilder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = clientBuilder
                    .WithService<ITeamService>(new StubTeamService(teams: stubTeam))
                    .Build();

                var content = new CreateTeamBuilder().BuildAsJsonContent();
                var response = await client.PostAsync("/api/teams", content);

                Assert.Equal(
                    expected: HttpStatusCode.Created,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task post_returns_expected_location_header()
        {
            using (var clientBuilder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = clientBuilder
                    .WithService<ITeamService>(new StubTeamService(teams: stubTeam))
                    .Build();

                var content = new CreateTeamBuilder().BuildAsJsonContent();
                var response = await client.PostAsync("/api/teams", content);

                Assert.EndsWith(
                    expectedEndString: $"/api/teams/{stubTeam.Id}",
                    actualString: string.Join("", response.Headers.Location.Segments)
                );
            }
        }

        [Fact]
        public async Task post_returns_expected_status_code_when_team_name_is_missing_from_input()
        {
            using (var clientBuilder = new HttpClientBuilder())
            {
                var client = clientBuilder
                    .WithService<ITeamService>(new StubTeamService())
                    .Build();

                var content = new CreateTeamBuilder()
                    .WithName(null)
                    .BuildAsJsonContent();

                var response = await client.PostAsync("/api/teams", content);

                Assert.Equal(
                    expected: HttpStatusCode.BadRequest,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task post_returns_expected_status_code_when_team_with_same_name_already_exists()
        {
            using (var clientBuilder = new HttpClientBuilder())
            {
                var client = clientBuilder
                    .WithService<ITeamService>(new StubTeamService(alreadyExists: true))
                    .Build();

                var content = new CreateTeamBuilder().BuildAsJsonContent();
                var response = await client.PostAsync("/api/teams", content);

                Assert.Equal(
                    expected: HttpStatusCode.Conflict,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task post_member_to_team_returns_expected_status_code_on_success()
        {
            using (var clientBuilder = new HttpClientBuilder())
            {
                var stubMember = new UserBuilder().Build();

                var stubTeam = new TeamBuilder()
                    .WithMembers(stubMember)
                    .Build();

                var client = clientBuilder
                    .WithService<ITeamService>(new StubTeamService(teams: stubTeam))
                    .Build();

                var response = await client.PostAsync(
                    requestUri: $"/api/teams/{stubTeam.Id}/members",
                    content: new JsonContent(new { UserId = stubMember.Id })
                );

                Assert.Equal(
                    expected: HttpStatusCode.OK,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task post_member_to_team_through_api_returns_expected_status_code_when_userid_is_missing()
        {
            using (var clientBuilder = new HttpClientBuilder())
            {
                var client = clientBuilder
                    .WithService(Dummy.Of<ITeamService>())
                    .Build();

                var dummyContent = JsonContent.Empty;
                var response = await client.PostAsync($"/api/teams/{Guid.NewGuid()}/members", dummyContent);

                Assert.Equal(
                    expected: HttpStatusCode.BadRequest,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task post_member_to_team_through_api_returns_expected_status_code_when_member_already_joined()
        {
            using (var clientBuilder = new HttpClientBuilder())
            {
                var client = clientBuilder
                    .WithService<ITeamService>(new ErroneousTeamService(new AlreadyJoinedException()))
                    .Build();

                var dummyContent = new JsonContent(new { UserId = 1 });
                var response = await client.PostAsync($"/api/teams/{Guid.NewGuid()}/members", dummyContent);

                Assert.Equal(
                    expected: HttpStatusCode.Conflict,
                    actual: response.StatusCode
                );
            }
        }
    }
}