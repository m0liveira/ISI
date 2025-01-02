using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using futFind.Models;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel;

public class SoapService : ISoapService
{
    private readonly AppDbContext _context;

    // Construtor que injeta o AppDbContext
    public SoapService(AppDbContext context)
    {
        _context = context;
    }

    public string ExportData(string tableName, string format)
    {
        try
        {
            // Recuperar dados reais do banco de dados, incluindo apenas jogos
            var data = tableName.ToLower() switch
            {
                "games" => _context.games
                    .Select(g => new 
                    {
                        GameId = g.id,
                        HostId = g.host_id, // ID do anfitrião
                        Date = g.date, // Data do jogo
                        Address = g.address, // Endereço do jogo
                        Capacity = g.capacity, // Capacidade do jogo
                        Price = g.price, // Preço do jogo
                        IsPrivate = g.is_private, // Se o jogo é privado
                        ShareCode = g.share_code, // Código de compartilhamento
                        Status = g.status // Status do jogo
                    }).ToList(),
                _ => throw new ArgumentException("Unsupported data type.")
            };

            if (format.ToLower() == "xml")
            {
                var xmlSerializer = new XmlSerializer(data.GetType());
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, data);
                    return stringWriter.ToString();
                }
            }
            else if (format.ToLower() == "csv")
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("GameId,HostId,Date,Address,Capacity,Price,IsPrivate,ShareCode,Status");
                foreach (var game in data)
                {
                    csvBuilder.AppendLine($"{game.GameId},{game.HostId},{game.Date},{game.Address},{game.Capacity},{game.Price},{game.IsPrivate},{game.ShareCode},{game.Status}");
                }
                return csvBuilder.ToString();
            }

            throw new ArgumentException("Unsupported format.");
        }
        catch (Exception ex)
        {
            // Log the exception (consider using a logging framework)
            return $"Error retrieving data: {ex.Message}";
        }
    }

    public string GenerateReport()
    {
        try
        {
            var totalGames = _context.games.Count(); // Total de jogos
            var totalPrivateGames = _context.games.Count(g => g.is_private); // Total de jogos privados
            var totalCapacity = _context.games.Sum(g => g.capacity) ?? 0; // Capacidade total
            var averagePrice = _context.games.Average(g => g.price) ?? 0; // Média de preços

            var report = new StringBuilder();
            report.AppendLine("Relatório de Jogos");
            report.AppendLine("-------------------");
            report.AppendLine($"Total de Jogos: {totalGames}");
            report.AppendLine($"Total de Jogos Privados: {totalPrivateGames}");
            report.AppendLine($"Capacidade Total: {totalCapacity}");
            report.AppendLine($"Preço Médio: {averagePrice}"); // Formatação de moeda

            return report.ToString();
        }
        catch (Exception ex)
        {
            // Log the exception (consider using a logging framework)
            return $"Error generating report: {ex.Message}";
        }
    }

    public string GetWeatherInfo(string city)
    {
        // Aqui chamar a API do OpenWeather
        // Exemplo fictício de retorno
        return $"O tempo em {city} está ensolarado com 25°C.";
    }
}