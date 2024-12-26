using System.ServiceModel;

[ServiceContract]
public interface ISoapService
{
    [OperationContract]
    string ExportData(string type, string format); // Para exportar dados em XML ou CSV

    [OperationContract]
    string GenerateReport(); // Para gerar um relatorio de uso

    [OperationContract]
    string GetWeatherInfo(string location); // Para obter informacoes meteorologicas
}