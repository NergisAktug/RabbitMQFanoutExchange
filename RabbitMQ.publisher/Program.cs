using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://mrltcrpj:nG85peo0qknQoavrqwK7OJ3NOB9GTr4T@baboon.rmq.cloudamqp.com/mrltcrpj");


using var connection = factory.CreateConnection();

//rabbitMq'ya bir kanal uzerinden baglanilir.
var channel = connection.CreateModel();

//durable :true demek ExchangeDeclare fiziksel olarak (belleğe) kayıt edilsin demek.
channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout);

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"Message {x}";

    //mesaj byte cevriliyor.Turkce karakterlerde sorun yasamamak icin
    var messageBody = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish("logs-fanout", "", null, messageBody);
    Console.WriteLine($"Sending messages: {message}");

});






Console.ReadKey();