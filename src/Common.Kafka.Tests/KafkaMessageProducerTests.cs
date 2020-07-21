using System.Threading;
using System.Threading.Tasks;
using Common.Kafka.Producer;
using Confluent.Kafka;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Common.Kafka.Tests
{
    public class KafkaMessageProducerTests
    {
        [Fact]
        public async Task ProduceShouldProduceMessageWithCorrectTopic()
        {
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder>();
            var mockProducer = new Mock<IProducer<string, string>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("some-key-id", "some-property-value");
            const string expectedTopic = "fake-messages";

            var sut = new KafkaMessageProducer(stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, CancellationToken.None);

            mockProducer.Verify(x => x.ProduceAsync(expectedTopic,
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ProduceShouldProduceMessageWithCorrectKey()
        {
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder>();
            var mockProducer = new Mock<IProducer<string, string>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("some-key-id", "some-property-value");

            var sut = new KafkaMessageProducer(stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, CancellationToken.None);

            mockProducer.Verify(x => x.ProduceAsync(It.IsAny<string>(),
                It.Is<Message<string, string>>(i => i.Key == fakeMessage.Key),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ProduceShouldProduceMessageWithSerialisedMessage()
        {
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder>();
            var mockProducer = new Mock<IProducer<string, string>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("some-key-id", "some-property-value");

            var sut = new KafkaMessageProducer(stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, CancellationToken.None);

            mockProducer.Verify(x => x.ProduceAsync(It.IsAny<string>(),
                It.Is<Message<string, string>>(i =>
                    i.Value == JsonConvert.SerializeObject(fakeMessage)),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ProduceShouldFlushProducer()
        {
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder>();
            var mockProducer = new Mock<IProducer<string, string>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("some-key-id", "some-property-value");

            var sut = new KafkaMessageProducer(stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, CancellationToken.None);

            mockProducer.Verify(x => x.Flush(It.IsAny<CancellationToken>()));
        }
    }
}