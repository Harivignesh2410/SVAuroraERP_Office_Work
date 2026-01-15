global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using Microsoft.Data.SqlClient;
global using Core.Logging.Models;
global using System.Data;
global using Core.Logging;
global using System.Linq.Dynamic.Core;
global using System.ComponentModel.DataAnnotations.Schema;


//Application
global using SVAuroraERP.Application.Interfaces;
global using SVAuroraERP.Application.Interfaces.Persistance.Authentication;
global using SVAuroraERP.Application.Interfaces.Persistance.Logger;
global using SVAuroraERP.Application.Interfaces.Persistance.HR;
global using SVAuroraERP.Application.Interfaces.Persistance.Production;
global using SVAuroraERP.Application.Interfaces.Persistance.Disptach;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.Invoice;

global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Disptach;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.MaterialInspection;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Purchase;

global using SVAuroraERP.Application.Interfaces.Persistance.Online.Master;
global using SVAuroraERP.Application.Interfaces.Persistance.Online.OEMVendorMapping;
global using SVAuroraERP.Application.Interfaces.Persistance.Dealer;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.Import;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.OrdersDelivery;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.ScrapManagement;

//Domain
global using SVAuroraERP.Domain.Authentication;
global using SVAuroraERP.Domain;
global using SVAuroraERP.Domain.Logging;
global using SVAuroraERP.Domain.Master;
global using SVAuroraERP.Domain.HR;
global using SVAuroraERP.Domain.Purchase;
global using SVAuroraERP.Domain.Production;
global using SVAuroraERP.Domain.Online.OEMVendorMapping;
global using SVAuroraERP.Domain.Orders.Import;
global using SVAuroraERP.Domain.Orders.Invoice;
global using SVAuroraERP.Domain.Orders.ManageOrder;

global using SVAuroraERP.Domain.Inventory.Dispatch;
global using SVAuroraERP.Domain.Inventory.Master;
global using SVAuroraERP.Domain.Inventory.Production;
global using SVAuroraERP.Domain.Inventory.MaterialInspection;
global using SVAuroraERP.Domain.Inventory.Purchase;
global using SVAuroraERP.Domain.Orders.OrdersDelivery;
global using SVAuroraERP.Domain.Online.Master;
global using SVAuroraERP.Domain.Dealer;


//Infrastructure
global using SVAuroraERP.Infrastructure.Repositories.Authentication;
global using SVAuroraERP.Infrastructure.Repositories.Master;
global using SVAuroraERP.Infrastructure.Repositories.HR;
global using SVAuroraERP.Infrastructure.Persistence;
global using SVAuroraERP.Infrastructure.Repositories.Inventory.Dispatch;
global using SVAuroraERP.Infrastructure.Repositories.Inventory.Master;
global using SVAuroraERP.Infrastructure.Repositories.Inventory.MaterialInspection;
global using SVAuroraERP.Infrastructure.Repositories.Inventory.Production;
global using SVAuroraERP.Infrastructure.Repositories.Inventory.Purchase;
global using SVAuroraERP.Infrastructure.Repositories.Online.Master;
global using SVAuroraERP.Infrastructure.Repositories.Online.OEMVendorMapping;
global using SVAuroraERP.Infrastructure.Repositories.Dealer;
global using SVAuroraERP.Infrastructure.Repositories.Orders.Import;
global using SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder;
global using SVAuroraERP.Infrastructure.Repositories.Orders.Import.Logger;
global using SVAuroraERP.Infrastructure.Repositories.Inventory.ScrapManagement;
global using SVAuroraERP.Infrastructure.Repositories.Orders.Invoice;
global using SVAuroraERP.Infrastructure.Repositories.Orders.OrdersDelivery;

