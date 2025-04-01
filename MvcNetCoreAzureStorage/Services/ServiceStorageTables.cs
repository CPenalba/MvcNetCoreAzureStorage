using Azure.Data.Tables;
using MvcNetCoreAzureStorage.Models;

namespace MvcNetCoreAzureStorage.Services
{
    public class ServiceStorageTables
    {
        private TableClient tableClient;

        public ServiceStorageTables(TableServiceClient tableService)
        {
            this.tableClient = tableService.GetTableClient("clientes");
        }

        public async Task CreateClientAsync(int id, string nombre, int salario, int edad, string empresa)
        {
            Cliente cliente = new Cliente();
            cliente.IdCliente = id;
            cliente.Nombre = nombre;
            cliente.Salario = salario;
            cliente.Edad = edad;
            cliente.Empresa = empresa;
            await this.tableClient.AddEntityAsync<Cliente>(cliente);
        }

        //INTERNAMENTE, SE PUEDEN BUSCAR CLIENTES POR CUAQUIER CAMPO
        //SI VAMOS A REALIZAR UNA BUSQUEDA, POR EJEMPLO PARA DETAILS, NO SE PUEDE BUSCAR SOLAMENTE
        //POR SU ROW KEY, SE GENERA UNA COMBINACION DE ROW KEY Y PARTITION KEY PARA BUSCAR POR ENTIDAD UNICA
        public async Task<Cliente> FindClientAsync(string partitionKey, string rowKey)
        {
            Cliente cliente = await this.tableClient.GetEntityAsync<Cliente>(partitionKey, rowKey);
            return cliente;
        }

        //PARA ELIMINAR UN ELEMENTO UNICO TAMBIEN NECESITAMOS PK Y RK
        public async Task DeleteClientAsync(string partitionKey, string rowKey)
        {
            await this.tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            List<Cliente> clientes = new List<Cliente>();
            //PARA BUSCAR, NECESITAMOS UTILIZAR UN OBJETO QUERY CON UN FILTER
            var query = this.tableClient.QueryAsync<Cliente>(filter: "");
            //DEBEMOS EXTRAER TODOS LOS DATOS DEL QUERY
            await foreach (var item in query)
            {
                clientes.Add(item);
            }

            return clientes;
        }

        public async Task<List<Cliente>> GetClientesEmpresaAsync(string empresa)
        {
            //TENEMOS DOS TIPOS DE FILTER, LOS DOS SE UTILIZAN CON query
            //1) SI REALIZAMOS EL FILTER CON QueryAsync,
            //DEBEMOS UTILIZAR UNA SINTAXIS Y EXTRAER LOS DATOS MANUALES
            //string filtro = "Campo eq valor";
            //string filtro = "Campo eq valor and Campo2 gt valor2";
            //string filtro = "Campo lt valor and Campo2 gt valor2";
            //string filtroSalario =
            //    "Salario gt 250000 and Salario lt 300000";
            //var query =
            //    this.tableClient.QueryAsync<Cliente>
            //    (filter: filtroSalario);

            //2) SI REALIZAMOS LA CONSULTA CON Query
            //PODEMOS UTILIZAR LAMBDA Y EXTRAER LOS DATOS DIRECTAMENTE, PERO NO ES ASINCRONO
            var query = this.tableClient.Query<Cliente>(x => x.Empresa == empresa);
            return query.ToList();
        }
    }
}
