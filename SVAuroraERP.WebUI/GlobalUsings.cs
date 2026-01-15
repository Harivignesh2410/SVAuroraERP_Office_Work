global using SVAuroraERP.Application;
global using SVAuroraERP.Infrastructure;
global using SVAuroraERP.WebUI.Custom;

global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Newtonsoft.Json;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Antiforgery;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.AspNetCore.SignalR;

global using Serilog;

global using System.Globalization;
global using System.Security.Claims;
global using System.ComponentModel.DataAnnotations;
global using System.Data;
global using ExcelDataReader;
global using ClosedXML.Excel;

global using SVAuroraERP.Domain.Online.OEMVendorMapping;
global using SVAuroraERP.Domain.Master;
global using SVAuroraERP.Domain.Authentication;
global using SVAuroraERP.Domain.HR;
global using SVAuroraERP.Domain.Purchase;
global using SVAuroraERP.Domain.Production;
global using SVAuroraERP.Domain.Inventory.Dispatch;
global using SVAuroraERP.Domain.Inventory.Master;
global using SVAuroraERP.Domain.Inventory.MaterialInspection;
global using SVAuroraERP.Domain.Inventory.Purchase;
global using SVAuroraERP.Domain.Inventory.Production;
global using SVAuroraERP.Domain.Orders.Invoice;
global using SVAuroraERP.Domain.Orders.ManageOrder;
global using SVAuroraERP.Domain.Online.Master;
global using SVAuroraERP.Domain;
global using SVAuroraERP.Domain.Orders.Import;
global using SVAuroraERP.Domain.Orders.OrdersDelivery;
global using SVAuroraERP.Domain.Inventory.ScrapManagement;

global using SVAuroraERP.Application.Interfaces.Persistance.Authentication;
global using SVAuroraERP.Application.Interfaces.Persistance.HR;
global using SVAuroraERP.Application.Interfaces.Persistance.Production;
global using SVAuroraERP.Application.Interfaces.Persistance.Disptach;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Disptach;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.MaterialInspection;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Purchase;
global using SVAuroraERP.Application.Interfaces.Persistance.Online.OEMVendorMapping;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production;
global using SVAuroraERP.Application.Interfaces.Persistance.Logger;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.Invoice;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.Import;
global using SVAuroraERP.Application.Interfaces.Persistance.Orders.OrdersDelivery;
global using SVAuroraERP.Application.Interfaces.Persistance.Online.Master;
global using SVAuroraERP.Application.Interfaces.Persistance.Inventory.ScrapManagement;


global using SVAuroraERP.Infrastructure.Repositories.Online.Master;
global using SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder;
