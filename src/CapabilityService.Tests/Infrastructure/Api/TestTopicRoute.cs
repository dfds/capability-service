using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Api
{
    public class TestTopicRoute
    {
        #region get all

        [Fact]
        public async Task get_all_topics_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository())
                    .Build();

                var response = await client.GetAsync("api/v1/topics");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task get_all_topics_returns_expected_body_when_NOT_having_any_topics()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository())
                    .Build();

                var response = await client.GetAsync("api/v1/topics");

                Assert.Equal(
                    expected: "{\"items\":[]}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task get_all_topics_returns_expected_body_when_having_single_topic()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTopic = new TopicBuilder().Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(stubTopic))
                    .Build();

                var response = await client.GetAsync("api/v1/topics");

                Assert.Equal(
                    expected: $"{{\"items\":[{{\"id\":\"{stubTopic.Id}\",\"name\":\"{stubTopic.Name}\",\"description\":\"{stubTopic.Description}\",\"isPrivate\":{stubTopic.IsPrivate.ToString().ToLower()},\"capabilityId\":\"{stubTopic.CapabilityId}\",\"messageContracts\":[]}}]}}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        #endregion

        #region get single topic

        [Fact]
        public async Task get_single_topic_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var dummyTopic = new TopicBuilder().Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(dummyTopic))
                    .Build();

                var response = await client.GetAsync("api/v1/topics/1");

                Assert.Equal(
                    expected: HttpStatusCode.OK,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task get_single_topic_returns_expected_body()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTopic = new TopicBuilder().Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(stubTopic))
                    .Build();

                var response = await client.GetAsync("api/v1/topics/1");

                Assert.Equal(
                    expected: $"{{\"id\":\"{stubTopic.Id}\",\"name\":\"{stubTopic.Name}\",\"description\":\"{stubTopic.Description}\",\"isPrivate\":{stubTopic.IsPrivate.ToString().ToLower()},\"capabilityId\":\"{stubTopic.CapabilityId}\",\"messageContracts\":[]}}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task get_single_topic_returns_expected_status_code_when_not_found()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository())
                    .Build();

                var response = await client.GetAsync("api/v1/topics/1");

                Assert.Equal(
                    expected: HttpStatusCode.NotFound,
                    actual: response.StatusCode
                );
            }
        }

        #endregion

        #region message contracts

        [Fact]
        public async Task get_message_contracts_for_single_topic_returns_expected_status_code_when_not_found()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository())
                    .Build();

                var response = await client.GetAsync("api/v1/topics/1/messagecontracts");

                Assert.Equal(
                    expected: HttpStatusCode.NotFound,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task get_message_contracts_for_single_topic_returns_expected_status_code_when_found()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTopic = new TopicBuilder().Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(stubTopic))
                    .Build();

                var response = await client.GetAsync($"api/v1/topics/{stubTopic.Id}/messagecontracts");

                Assert.Equal(
                    expected: HttpStatusCode.OK,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task get_message_contracts_for_single_topic_returns_expected_body_when_empty()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTopic = new TopicBuilder().Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(stubTopic))
                    .Build();

                var response = await client.GetAsync($"api/v1/topics/{stubTopic.Id}/messagecontracts");

                Assert.Equal(
                    expected: "{\"items\":[]}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task get_message_contracts_for_single_topic_returns_expected_body()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubMessageContract = new MessageContractBuilder()
                    .WithType("foo")
                    .WithContent("bar")
                    .WithDescription("baz")
                    .Build();

                var stubTopic = new TopicBuilder()
                    .WithMessageContracts(stubMessageContract)
                    .Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(stubTopic))
                    .Build();

                var response = await client.GetAsync($"api/v1/topics/{stubTopic.Id}/messagecontracts");

                Assert.Equal(
                    expected: $"{{\"items\":[{{\"type\":\"{stubMessageContract.Type}\",\"content\":\"{stubMessageContract.Content}\",\"description\":\"{stubMessageContract.Description}\"}}]}}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task post_message_contract_to_NON_EXISTING_topic_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var emptyStubTopicRepository = new StubTopicRepository();

                var client = builder
                    .WithService<ITopicRepository>(emptyStubTopicRepository)
                    .Build();

                var nonExistingTopicId = 1;
                var stubInput = "{\"type\":\"foo\",\"description\":\"bar\",\"content\":\"baz\"}";

                var response = await client.PostAsync($"api/v1/topics/{nonExistingTopicId}/messagecontracts", new JsonContent(stubInput));

                Assert.Equal(
                    expected: HttpStatusCode.NotFound,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task post_message_contract_to_topic_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTopic = new TopicBuilder()
                    .Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(stubTopic))
                    .Build();

                var stubInput = "{\"type\":\"foo\",\"description\":\"bar\",\"content\":\"baz\"}";

                var response = await client.PostAsync($"api/v1/topics/{stubTopic.Id}/messagecontracts", new JsonContent(stubInput));

                Assert.Equal(
                    expected: HttpStatusCode.NoContent,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task delete_message_contract_from_NON_EXISTING_topic_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var emptyStubTopicRepository = new StubTopicRepository();

                var client = builder
                    .WithService<ITopicRepository>(emptyStubTopicRepository)
                    .Build();

                var nonExtingTopicId = "foo";
                var dummyMessageContractId = "dummy-id";

                var response = await client.DeleteAsync($"api/v1/topics/{nonExtingTopicId}/messagecontracts/{dummyMessageContractId}");

                Assert.Equal(
                    expected: HttpStatusCode.NotFound,
                    actual: response.StatusCode
                );
            }
        }

        [Fact]
        public async Task delete_NON_EXISTING_message_contract_from_topic_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var stubTopic = new TopicBuilder().Build();

                var client = builder
                    .WithService<ITopicRepository>(new StubTopicRepository(stubTopic))
                    .Build();

                var nonExistingMessageId = "foo";

                var response = await client.DeleteAsync($"api/v1/topics/{stubTopic.Id}/messagecontracts/{nonExistingMessageId}");

                Assert.Equal(
                    expected: HttpStatusCode.NoContent,
                    actual: response.StatusCode
                );
            }
        }

        // [x] GET, when topic NOT found => status code
        // [x] GET, when topic found => status code
        // [x] GET, when topic found => body when empty
        // [x] GET, when topic found => body when single
           
        // [x] POST, when topic NOT found => status code
        // [x] POST, when topic found => status code
        
        // [x] DELETE, when topic NOT found => status code
        // [x] DELETE, when topic found => status code

        #endregion
    }
}