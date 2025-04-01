using Microsoft.AspNetCore.Mvc;
using MvcNetCoreAzureStorage.Models;
using MvcNetCoreAzureStorage.Services;

namespace MvcNetCoreAzureStorage.Controllers
{
    public class AzureTablesController : Controller
    {
        private ServiceStorageTables service;

        public AzureTablesController(ServiceStorageTables service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<Cliente> clientes = await this.service.GetClientesAsync();
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            await this.service.CreateClientAsync(cliente.IdCliente, cliente.Nombre, cliente.Salario, cliente.Edad, cliente.Empresa);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string partitionkey, string rowkey)
        {
            await this.service.DeleteClientAsync(partitionkey, rowkey);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string partitionkey, string rowkey)
        {
            Cliente cliente = await this.service.FindClientAsync(partitionkey, rowkey);
            return View(cliente);
        }

        public IActionResult ClientesEmpresa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ClientesEmpresa(string empresa)
        {
            List<Cliente> clientes = await this.service.GetClientesEmpresaAsync(empresa);
            return View(clientes);
        }
    }
}
