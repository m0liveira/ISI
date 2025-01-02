using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using futFind.Models;
using futFind.Services;
using Microsoft.EntityFrameworkCore;

public class SoapServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly SoapService _soapService;

    public SoapServiceTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _soapService = new SoapService(_mockContext.Object);
    }

    [Fact]
    public void ExportData_ShouldReturnXml_WhenFormatIsXml()
    {
        // Arrange
        var games = new List<Games>
        {
            new Games { id = 1, host_id = 1, date = DateTime.Now, address = "Address 1", capacity = 10, price = 20.5m, is_private = false, share_code = "ABC123", status = "Scheduled" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Games>>();
        mockSet.As<IQueryable<Games>>().Setup(m => m.Provider).Returns(games.Provider);
        mockSet.As<IQueryable<Games>>().Setup(m => m.Expression).Returns(games.Expression);
        mockSet.As<IQueryable<Games>>().Setup(m => m.ElementType).Returns(games.ElementType);
        mockSet.As<IQueryable<Games>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());

        _mockContext.Setup(c => c.games).Returns(mockSet.Object);

        // Act
        var result = _soapService.ExportData("games", "xml");

        // Assert
        Assert.Contains("<Games>", result);
    }

    [Fact]
    public void ExportData_ShouldReturnCsv_WhenFormatIsCsv()
    {
        // Arrange
        var games = new List<Games>
        {
            new Games { id = 1, host_id = 1, date = DateTime.Now, address = "Address 1", capacity = 10, price = 20.5m, is_private = false, share_code = "ABC123", status = "Scheduled" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Games>>();
        mockSet.As<IQueryable<Games>>().Setup(m => m.Provider).Returns(games.Provider);
        mockSet.As<IQueryable<Games>>().Setup(m => m.Expression).Returns(games.Expression);
        mockSet.As<IQueryable<Games>>().Setup(m => m.ElementType).Returns(games.ElementType);
        mockSet.As<IQueryable<Games>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());

        _mockContext.Setup(c => c.games).Returns(mockSet.Object);

        // Act
        var result = _soapService.ExportData("games", "csv");

        // Assert
        Assert.Contains("GameId,HostId,Date,Address,Capacity,Price,IsPrivate,ShareCode,Status", result);
        Assert.Contains("1,1,", result); // Check for the first game's data
    }

    [Fact]
    public void ExportData_ShouldThrowException_WhenTypeIsUnsupported()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _soapService.ExportData("unsupportedType", "xml"));
        Assert.Equal("Unsupported data type.", exception.Message);
    }

    [Fact]
    public void ExportData_ShouldThrowException_WhenFormatIsUnsupported()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _soapService.ExportData("games", "unsupportedFormat"));
        Assert.Equal("Unsupported format.", exception.Message);
    }

    [Fact]
    public void ExportData_ShouldReturnErrorMessage_WhenExceptionOccurs()
    {
        // Arrange
        _mockContext.Setup(c => c.games).Throws(new Exception("Database error"));

        // Act
        var result = _soapService.ExportData("games", "xml");

        // Assert
        Assert.Contains("Error retrieving data: Database error", result);
    }

    [Fact]
    public void GenerateReport_ShouldReturnCorrectReport()
    {
        // Arrange
        var games = new List<Games>
        {
            new Games { id = 1, host_id = 1, date = DateTime.Now, address = "Address 1", capacity = 10, price = 20.5m, is_private = false, share_code = "ABC123", status = "Scheduled" },
            new Games { id = 2, host_id = 2, date = DateTime.Now, address = "Address 2", capacity = 20, price = 30.5m, is_private = true, share_code = "DEF456", status = "Scheduled" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Games>>();
        mockSet.As<IQueryable<Games>>().Setup(m => m.Provider).Returns(games.Provider);
        mockSet.As<IQueryable<Games>>().Setup(m => m.Expression).Returns(games.Expression);
        mockSet.As<IQueryable<Games>>().Setup(m => m.ElementType).Returns(games.ElementType);
        mockSet.As<IQueryable<Games>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());

        _mockContext.Setup(c => c.games).Returns(mockSet.Object);

        // Act
        var result = _soapService.GenerateReport();

        // Assert
        Assert.Contains("Total de Jogos: 2", result);
        Assert.Contains("Total de Jogos Privados: 1", result);
        Assert.Contains("Capacidade Total: 30", result);
        Assert.Contains("Preço Médio: R$25,50", result); // Adjust currency format as needed
    }

    [Fact]
    public void GetWeatherInfo_ShouldReturnWeatherInfo()
    {
        // Act
        var result = _soapService.GetWeatherInfo("Lisboa");

        // Assert
        Assert.Contains("O tempo em Lisboa é ensolarado com 25°C.", result);
    }
}
