using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using Xunit;

namespace DFDS.TeamService.Tests.Features.Teams
{
    public class TestTeam
    {
        [Fact]
        public void returns_expected_list_of_members_when_initialized()
        {
            var sut = new TeamBuilder().Build();
            Assert.Empty(sut.Members);
        }

        [Fact]
        public void returns_expected_list_of_members_after_single_membership_has_been_started()
        {
            var sut = new TeamBuilder().Build();
            var stubUser = new UserBuilder().Build();

            sut.StartMembership(
                user: stubUser,
                membershipType: MembershipType.Unknown
            );

            Assert.Equal(new[] {stubUser}, sut.Members);
        }

        [Fact]
        public void returns_expected_list_of_members_after_multiple_memberships_has_been_started()
        {
            var sut = new TeamBuilder().Build();

            var expected = new[]
            {
                new UserBuilder()
                    .WithId("1")
                    .Build(),
                new UserBuilder()
                    .WithId("2")
                    .Build()
            };

            foreach (var stubUser in expected)
            {
                sut.StartMembership(
                    user: stubUser,
                    membershipType: MembershipType.Unknown
                );
                
            }
            Assert.Equal(expected, sut.Members);
        }

        [Fact]
        public void returns_expected_list_of_members_after_same_user_starts_membership_with_different_roles()
        {
            var sut = new TeamBuilder().Build();
            var stubUser = new UserBuilder().Build();

            sut.StartMembership(stubUser, MembershipType.Unknown);
            sut.StartMembership(stubUser, MembershipType.Developer);

            Assert.Equal(new[] { stubUser }, sut.Members);
        }

        [Fact]
        public void returns_expected_when_finding_a_member_that_is_not_part_of_the_team()
        {
            var sut = new TeamBuilder().Build();
            var user = sut.FindMemberById("non-existing-member-id");

            Assert.Null(user);
        }

        [Fact]
        public void returns_expected_when_finding_a_member_that_is_part_of_the_team()
        {
            var stubUser = new UserBuilder().Build();

            var sut = new TeamBuilder()
                .WithMembers(stubUser)
                .Build();

            var result = sut.FindMemberById(stubUser.Id);

            Assert.Equal(stubUser, result);
        }

        [Fact]
        public void returns_expected_when_finding_a_member_that_has_multiple_memberships_of_the_team()
        {
            var sut = new TeamBuilder().Build();

            var stubUser = new UserBuilder().Build();
            sut.StartMembership(stubUser, MembershipType.Unknown);
            sut.StartMembership(stubUser, MembershipType.Developer);

            var result = sut.FindMemberById(stubUser.Id);

            Assert.Equal(stubUser, result);
        }

        [Fact]
        public void returns_expected_name_when_initialized()
        {
            var expectedName = "foo-bar";

            var sut = new TeamBuilder()
                .WithName(expectedName)
                .Build();

            Assert.Equal(expectedName, sut.Name);
        }

        [Fact]
        public void returns_expected_name_when_changed()
        {
            var expectedName = "foo";

            var sut = new TeamBuilder()
                .WithName("bar")
                .Build();

            sut.ChangeName(expectedName);

            Assert.Equal(expectedName, sut.Name);
        }

        [Fact]
        public void returns_expected_department_when_initialized()
        {
            var expectedName = "foo-bar";

            var sut = new TeamBuilder()
                .WithDepartment(expectedName)
                .Build();

            Assert.Equal(expectedName, sut.Department);
        }

        [Fact]
        public void returns_expected_department_when_changed()
        {
            var expectedName = "foo";

            var sut = new TeamBuilder()
                .WithDepartment("bar")
                .Build();

            sut.ChangeDepartment(expectedName);

            Assert.Equal(expectedName, sut.Department);
        }
    }
}