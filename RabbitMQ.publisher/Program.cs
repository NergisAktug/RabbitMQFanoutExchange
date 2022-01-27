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
            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            //** Herbir Enum(queue message) foreach ile isimine gore ve direct exchange ile gonderiliyor. 
            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                Random rnd = new Random();
                LogNames log1 =(LogNames) rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);
                var rootKey = $"{log1}.{log2}.{log3}";
            });

            Random rnd = new Random();
            //rootlama islemi
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log=(LogNames)new Random().Next(1,5);
        
                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);
                var rootKey = $"{log1}.{log2}.{log3}";
                string message = $"Log-type: {log1}-{log2}-{log3}";
                //mesaj byte cevriliyor.Turkce karakterlerde sorun yasamamak icin
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-topic",rootKey, null, messageBody);
                Console.WriteLine($"Sending Log: {message}");

            });


            Console.ReadKey();
        }
    } 
}


    
