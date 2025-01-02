using System;
using System.ServiceModel;
using Xunit;

public class SoapClientTests
{
    private readonly ISoapService _client;

    public SoapClientTests()
    {
        var binding = new BasicHttpBinding();
        var endpoint = new EndpointAddress("http://localhost:5007/SoapService.svc"); // Use HTTP se HTTPS não estiver configurado
        var channelFactory = new ChannelFactory<ISoapService>(binding, endpoint);
        _client = channelFactory.CreateChannel();
    }

    [Fact]
    public void ExportData_ShouldReturnXml()
    {
        // Act
        var result = _client.ExportData("games", "xml");

        // Assert
        Assert.Contains("<ArrayOfAnonymousType", result);
    }

    [Fact]
    public void ExportData_ShouldReturnCsv()
    {
        // Act
        var result = _client.ExportData("games", "csv");

        // Assert
        Assert.Contains("GameId,HostId,Date,Address,Capacity,Price,IsPrivate,ShareCode,Status", result);
    }

    [Fact]
    public void GenerateReport_ShouldReturnReport()
    {
        // Act
        var result = _client.GenerateReport();

        // Assert
        Assert.Contains("Relatório de Jogos", result);
    }

    [Fact]
    public void GetWeatherInfo_ShouldReturnWeatherInfo()
    {
        // Act
        var result = _client.GetWeatherInfo("Lisboa");

        // Assert
        Assert.Contains("O tempo em Lisboa é ensolarado com 25°C.", result);
    }
}
