using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.Tests.TestDoubles;
using DFDS.TeamService.WebApi;
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
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService())
                    .Build();

                var response = await client.GetAsync("api/v1/teams");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task get_all_teams_returns_expected_body_when_no_teams_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService())
                    .Build();

                var response = await client.GetAsync("api/v1/teams");

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
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                    .Build();

                var response = await client.GetAsync($"api/v1/teams/{stubTeam.Id}");

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
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                    .Build();

                var response = await client.GetAsync($"api/v1/teams/{stubTeam.Id}");

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
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                    .Build();

                var response = await client.GetAsync($"api/v1/teams/{stubTeam.Id}");

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
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService())
                    .Build();

                var unknownTeamId = Guid.Empty;
                var response = await client.GetAsync($"api/v1/teams/{unknownTeamId}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task get_single_team_returns_expected_status_code_when_input_teamid_is_invalid()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService())
                    .Build();

                var invalidTeamId = "im-not-a-valid-team-id";
                var response = await client.GetAsync($"api/v1/teams/{invalidTeamId}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task post_returns_expected_status_code_when_creating_a_new_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var dummyTeam = new TeamBuilder().Build();

                var client = builder
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService(dummyTeam))
                    .Build();

                var stubInput = "{\"name\":\"foo\"}";
                var response = await client.PostAsync("api/v1/teams", new JsonContent(stubInput));

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }

        [Fact]
        public async Task post_returns_expected_location_header_when_creating_a_new_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = builder
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                    .Build();

                var stubInput = "{\"name\":\"foo\"}";
                var response = await client.PostAsync("api/v1/teams", new JsonContent(stubInput));

                Assert.EndsWith(
                    expectedEndString: $"/api/v1/teams/{stubTeam.Id}",
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
                     .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                     .Build();

                var stubInput = $"{{\"name\":\"{stubTeam.Name}\"}}";
                var response = await client.PostAsync("api/v1/teams", new JsonContent(stubInput));

                Assert.Equal(
                    expected: $"{{\"id\":\"{stubTeam.Id}\",\"name\":\"{stubTeam.Name}\",\"members\":[]}}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task post_returns_badrequest_when_creating_a_new_team_with_invalid_name()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var errorText = "message regarding bad name";
                var client = builder
                     .WithService<ITeamApplicationService>(new ErroneousTeamApplicationService(new TeamValidationException(errorText)))
                     .Build();

                var stubInput = $"{{\"name\":\"{stubTeam.Name}\"}}";
                var response = await client.PostAsync("api/v1/teams", new JsonContent(stubInput));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Equal(
                    expected: errorText,
                    actual: await response.Content.ReadAsStringAsync()
                );

            }
        }

        [Fact]
        public async Task post_returns_expected_status_code_when_adding_a_member_to_an_existing_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder().Build();

                var client = builder
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                    .Build();

                var stubInput = "{\"email\":\"foo\"}";
                var response = await client.PostAsync($"api/v1/teams/{stubTeam.Id}/members", new JsonContent(stubInput));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }     
        }

        [Fact]
        public async Task delete_returns_expected_status_code_when_deleting_an_existing_member_on_a_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTeam = new TeamBuilder()
                    .WithMembers("foo")
                    .Build();

                var client = builder
                    .WithService<ITeamApplicationService>(new StubTeamApplicationService(stubTeam))
                    .Build();

                var response = await client.DeleteAsync($"api/v1/teams/{stubTeam.Id}/members/foo");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task delete_returns_expected_status_code_when_deleting_a_non_existing_member_on_a_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamApplicationService>(new ErroneousTeamApplicationService(new NotMemberOfTeamException()))
                    .Build();

                var dummyTeamId = "foo";
                var dummyMemberEmail = "bar";

                var response = await client.DeleteAsync($@"api/v1/teams/{dummyTeamId}/members/{dummyMemberEmail}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task delete_returns_expected_status_code_when_deleting_a_member_on_a_non_existing_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITeamApplicationService>(new ErroneousTeamApplicationService(new TeamDoesNotExistException()))
                    .Build();

                var dummyTeamId = "foo";
                var dummyMemberEmail = "bar";

                var response = await client.DeleteAsync($@"api/v1/teams/{dummyTeamId}/members/{dummyMemberEmail}");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task post_returns_expected_status_code_when_adding_a_member_to_a_non_existing_team()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                             .WithService<ITeamApplicationService>(new ErroneousTeamApplicationService(new TeamDoesNotExistException()))
                             .Build();

                var nonExistingTeamId = "foo";
                var dummyInput = "{}";
                var response = await client.PostAsync($"api/v1/teams/{nonExistingTeamId}/members", new JsonContent(dummyInput));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }     
        }
    }
}