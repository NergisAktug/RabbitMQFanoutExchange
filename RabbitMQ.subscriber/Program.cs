using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://mrltcrpj:nG85peo0qknQoavrqwK7OJ3NOB9GTr4T@baboon.rmq.cloudamqp.com/mrltcrpj");


using var connection = factory.CreateConnection();

//rabbitMq'ya bir kanal uzerinden baglanilir.
var channel = connection.CreateModel();


var randomQueueName = channel.QueueDeclare().QueueName;//Kanal uzerindeki queue'lardan random bir kuyruk ismi  verir.
//channel.QueueDeclare(randomQueueName, true, false, false);channel uzerinde QueueDeclare edersek ilgili subscriber(ilgili instance) oluşsa dahi bu kuyruk durur 

channel.QueueBind(randomQueueName, "logs-fanout","",null);//QueueBind ile bearber uygulama her ayaga kalktıgında bir kuyruk oluşacak ama uygulama down oldugunda uygulama silinecek.    root kısmı şuanda boş


//her bir subscriber'a kac mesaj gelecek bunu ayarlamak icin:
channel.BasicQos(0, 1, false);//global parametresinin true olması tek bir seferde subscriberlara gonderilcek toplam queue sayısı doğrular:Yani 5 queue gonderilecekce bunun 2 Tanesi a sbuscribera gonderiri 3'sunu B subscriber'a gonderir.
//BasicQos(0,5,false) global'ı false olması tek seferde A subscriber ve B subscriber'lara 5 er queue gonderir.
var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume(randomQueueName, false, consumer);//Bir kuyruk ismi istiyor.Bir sonraki parametre autoAck mesaj ulastıktan sonra silinmesi isteniyorsa true isaretlenir.


Console.WriteLine("Listening to logs...");
consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Thread.Sleep(1500);

    Console.WriteLine("Incoming messages:" + message);
    //DeliveryTag ile bana ulasılan şu taglı mesajı RabbitMQ'ya gonderiyorum,RabbitMq'da ilgili mesajı kuruktan siliyor.
    channel.BasicAck(e.DeliveryTag, false);//multiple parametresi true yapılırsa bunun gibi baska işlenmiş mesajlar var ise memory de onlarıda kuyruktan silmeye yarar.
};



Console.ReadKey();