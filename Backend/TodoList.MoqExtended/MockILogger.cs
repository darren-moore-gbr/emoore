using Microsoft.Extensions.Logging;
using Moq;

namespace TodoList.MoqExtended
{
    public class MockILogger<T> : Mock<ILogger<T>> where T : class
    {
        public void VerifyLogger(Func<Times> times)
        {
            VerifyLogger(It.IsAny<LogLevel>(), times);
        }

        public void VerifyLogger(LogLevel logLevel, Func<Times> times)
        {
            Verify(x =>
                x.Log(logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), times);
        }
    }
}
