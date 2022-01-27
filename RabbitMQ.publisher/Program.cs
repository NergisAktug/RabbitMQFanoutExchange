using RabbitMQ.Client;
using System.Text;
using System;
using Shared;
using System.Text.Json;

namespace RabbitMQ.publisher
{
  
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
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string,object> headers= new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            var properties=channel.CreateBasicProperties();
            properties.Headers=headers;
            properties.Persistent = true;//mesajları kalıcı hale getirir

            var product = new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 10 };
            var productJsonString=JsonSerializer.Serialize(product);

            //channel.BasicPublish("header-exchange", string.Empty,properties,Encoding.UTF8.GetBytes("my header message"));//Artık root uzerinden degil header uzerinden gerceklestigi icin 2.parametreye empty degeri verilmisitr.
            channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));//Artık root uzerinden degil header uzerinden gerceklestigi icin 2.parametreye empty degeri verilmisitr.

            Console.WriteLine("Message Sent");
            Console.ReadKey();
        }
    } 
}


    
