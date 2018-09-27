﻿using System;
using System.Collections.Generic;
using Autofac;
using AutoMapper;
using Domain0.Nancy.Infrastructure;
using Domain0.Nancy.Service;
using Domain0.Service;
using Microsoft.IdentityModel.Tokens;

namespace Domain0
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var settings = new TokenGeneratorSettings
            {
                Audience = "*",
                Issuer = "issuer",
                Lifetime = TimeSpan.FromMinutes(15),
                Secret = "kiHLSfGebYvXGTDx0vWb53JhyUpnw6HvgRwOJ6h/hUs=",
                Alg = SecurityAlgorithms.HmacSha256,
            };
            builder.RegisterInstance(settings);
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().SingleInstance();

            builder.RegisterType<MapperProfile>().As<Profile>().SingleInstance();
            builder.RegisterType<PasswordGenerator>().As<IPasswordGenerator>().SingleInstance();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<AdminService>().As<IAdminService>().InstancePerLifetimeScope();

            builder.RegisterInstance(new SqlQueueSmsClientSettings
            {
                ConnectionString = "Data Source=WS-00009\\SQLEXPRESS;Initial Catalog=CrmNotification;Integrated Security=True",
                QueueName = "SmsOutcoming"
            });
            builder.RegisterType<SqlQueueSmsClient>().As<ISmsClient>();
            builder
                .RegisterType<AuthenticationConfigurationBuilder>()
                .As<IAuthenticationConfigurationBuilder>()
                .SingleInstance();

            builder.RegisterInstance(new EmailClientSettings
            {
                ServerHost = "smtp.gmail.com",
                Port = 587,
                Email = "",
                Username = "",
                Password = ""
            });
            builder.RegisterType<EmailClient>().As<IEmailClient>();

            builder.Register(container =>
            {
                var profiles = container.Resolve<IEnumerable<Profile>>();
                var mapper = new MapperConfiguration(c =>
                {
                    foreach (var profile in profiles)
                        c.AddProfile(profile);
                }).CreateMapper();
                return mapper;

            }).As<IMapper>().SingleInstance();
        }
    }
}
