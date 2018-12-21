using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.Tests.TestDoubles;
using DFDS.TeamService.WebApi.Models;
using Xunit;

namespace DFDS.TeamService.Tests
{
    public class TestTeamsRoute
    {
        [Fact]
        public async Task get_all_teams_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository())
                    .Build();

                var response = await client.GetAsync("api/teams");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task get_all_teams_returns_expected_body_when_no_teams_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository())
                    .Build();

                var response = await client.GetAsync("api/teams");

                Assert.Equal(
                    expected: "{\"items\":[]}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task get_single_team_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository(stubTeam))
                    .Build();

                var response = await client.GetAsync($"api/teams/{stubTeam.Id}");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task get_single_team_without_any_members_returns_expected_body()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository(stubTeam))
                    .Build();

                var response = await client.GetAsync($"api/teams/{stubTeam.Id}");

                Assert.Equal(
                    expected: $"{{\"id\":\"{stubTeam.Id}\",\"name\":\"{stubTeam.Name}\",\"members\":[]}}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task get_single_team_with_a_single_member_returns_expected_body()
        {
            using (var builder = new HttpClientBuilder())
            {
                var memberEmail = "foo@bar.com";

                var stubTeam = new TeamBuilder()
                    .WithMembers(memberEmail)
                    .Build();

                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository(stubTeam))
                    .Build();

                var response = await client.GetAsync($"api/teams/{stubTeam.Id}");

                Assert.Equal(
                    expected: $"{{\"id\":\"{stubTeam.Id}\",\"name\":\"{stubTeam.Name}\",\"members\":[{{\"email\":\"{memberEmail}\"}}]}}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task get_single_team_returns_expected_status_code_when_team_was_not_found()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository())
                    .Build();

                var unknownTeamId = Guid.Empty;
                var response = await client.GetAsync($"api/teams/{unknownTeamId}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task get_single_team_returns_expected_status_code_when_input_teamid_is_invalid()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository())
                    .Build();

                var invalidTeamId = "im-not-a-valid-team-id";
                var response = await client.GetAsync($"api/teams/{invalidTeamId}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task post_returns_expected_status_code_when_creating_a_new_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository())
                    .Build();

                var stubInput = "{\"name\":\"foo\"}";
                var response = await client.PostAsync("api/teams", new StringContent(stubInput, Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }

        [Fact]
        public async Task post_returns_expected_location_header_when_creating_a_new_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamRepository>(new StubTeamRepository())
                    .Build();

                var stubInput = "{\"name\":\"foo\"}";
                var response = await client.PostAsync("api/teams", new StringContent(stubInput, Encoding.UTF8, "application/json"));

                Assert.Matches(
                    expectedRegexPattern: @"^(https?://.+?)?/api/teams/.+$",
                    actualString: response.Headers.Location.ToString()
                );
            }
        }

        [Fact]
        public async Task post_returns_expected_body_when_creating_a_new_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = builder
                     .WithService<ITeamRepository>(Dummy.Of<ITeamRepository>())
                     .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                     .Build();

                var stubInput = $"{{\"name\":\"{stubTeam.Name}\"}}";
                var response = await client.PostAsync("api/teams", new StringContent(stubInput, Encoding.UTF8, "application/json"));

                Assert.Equal(
                    expected: $"{{\"id\":\"{stubTeam.Id}\",\"name\":\"{stubTeam.Name}\",\"members\":[]}}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }
    }
}