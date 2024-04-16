using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class Street
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int StreetIndex { get; set; }
}

public class MyDbContext : DbContext
{
    public DbSet<Street> Street { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = @"data source=DESKTOP-JGNME93;
                                    initial catalog=DbStreet;
                                    integrated security=True;
                                    Connect Timeout=2;
                                    Encrypt=False;
                                    Trust Server Certificate=False;
                                    Application Intent=ReadWrite;
                                    Multi Subnet Failover=False";

            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.EnableRetryOnFailure();
            });
        }
    }

}

class Program
{
    private static void Main(string[] args)
    {
        string address = "127.0.00.1";
        int port = 8080;
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
        IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);

        UdpClient listener = new UdpClient(ipPoint);

        try
        {
            Console.WriteLine("Server started! Waiting for connection .... ");

            while (true)
            {
                int bytes = 0;
                byte[] data = listener.Receive(ref remotePoint);
                string msg = Encoding.Unicode.GetString(data);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} :: {msg}");


                int streetIndex = int.Parse(msg);
                string message = "";
                MyDbContext db = new MyDbContext();
                var streets = db.Street
                            .Where(e => e.StreetIndex == streetIndex)
                            .Select(e => e.Name)
                            .ToList();


                foreach (var street in streets)
                {
                    Console.WriteLine($"Street: {street}");
                    message += street + '\n';
                }
                

        
                data = Encoding.Unicode.GetBytes(message);
                listener.Send(data, data.Length, remotePoint);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.ToString()}");
        }
    }
}
