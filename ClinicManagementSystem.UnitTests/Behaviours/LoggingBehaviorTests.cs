using ClinicManagementSystem.Application.Common.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagementSystem.UnitTests.Behaviours
{
    public class LoggingBehaviorTests
    {
        private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> _loggerMock;
        private readonly LoggingBehavior<TestRequest, TestResponse> _behavior;

        public LoggingBehaviorTests()
        {
            _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
            _behavior = new LoggingBehavior<TestRequest, TestResponse>(_loggerMock.Object);
        }

        public record TestRequest : IRequest<TestResponse>;
        public record TestResponse(string Value);

        [Fact]
        public async Task Handle_WhenRequestSucceeds_LogsInformationTwice()
        {
            var request = new TestRequest();
            var response = new TestResponse("Success");

            RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(response);

            var result = await _behavior.Handle(request, next, CancellationToken.None);

            result.Should().Be(response);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handled")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRequestThrows_LogsErrorAndRethrows()
        {
            var request = new TestRequest();
            var exception = new Exception("Something failed");

            RequestHandlerDelegate<TestResponse> next = (ct) => throw exception;

            var act = async () => await _behavior.Handle(request, next, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>().WithMessage("Something failed");

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error handling")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRequestSucceeds_CallsNextDelegate()
        {
            var request = new TestRequest();
            var response = new TestResponse("Success");
            var nextCalled = false;

            RequestHandlerDelegate<TestResponse> next = (ct) =>
            {
                nextCalled = true;
                return Task.FromResult(response);
            };

            await _behavior.Handle(request, next, CancellationToken.None);

            nextCalled.Should().BeTrue();
        }
    }
}
