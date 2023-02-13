﻿global using Autofac.Extensions.DependencyInjection;
global using Autofac;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.AspNetCore;
global using Azure.Messaging.ServiceBus;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
global using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Logging;
global using RabbitMQ.Client;
global using Serilog.Context;
global using Serilog;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO;
global using System.Reflection;
global using System.Threading.Tasks;
global using System;
