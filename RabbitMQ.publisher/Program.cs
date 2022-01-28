using RabbitMQ.Client;
using System.Text;
using System;

namespace RabbitMQ.publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4

    }

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://mrltcrpj:nG85peo0qknQoavrqwK7OJ3NOB9GTr4T@baboon.rmq.cloudamqp.com/mrltcrpj");


            using var connection = factory.CreateConnection();
            //rabbitMq'ya bir kanal uzerinden baglanilir.
            var channel = connection.CreateModel();
            //durable :true demek ExchangeDeclare fiziksel olarak (belleğe) kayıt edilsin demek.
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            //** Herbir Enum(queue message) foreach ile isimine gore ve direct exchange ile gonderiliyor. 
            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var rootKey = $"root-{x}";
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName, durable:true,false,false);
                channel.QueueBind(queueName, "logs-direct",rootKey);
            });


            //rootlama islemi
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log=(LogNames)new Random().Next(1,5);
                string message = $"Log-type: {log}";

                //mesaj byte cevriliyor.Turkce karakterlerde sorun yasamamak icin
                var messageBody = Encoding.UTF8.GetBytes(message);

                var rootKey = $"root-{log}";

                channel.BasicPublish("logs-direct",rootKey, null, messageBody);
                Console.WriteLine($"Sending Log: {message}");

            });


            Console.ReadKey();
        }
    } 
}


    
