﻿using Autofac;
using Autofac.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Web;


namespace Bot_Application1.Exceptions
{
    public class DefaultExceptionMessageOverrideModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PostUnhandledExceptionToUser>().Keyed<IPostToBot>(typeof(PostUnhandledExceptionToUser)).InstancePerLifetimeScope();

            try
            {
                RegisterAdapterChain<IPostToBot>(builder,
                    typeof(PersistentDialogTask),
                    typeof(ExceptionTranslationDialogTask),
                    typeof(SerializeByConversation),
                    typeof(SetAmbientThreadCulture),
                    typeof(PostUnhandledExceptionToUser),
                    typeof(LogPostToBot)
                )
                .InstancePerLifetimeScope();
            }
            catch (Exception error)
            {
                Logger.addErrorLog("DefaultExceptionMessageOverrideModule", error.Message + error.Data + error.Source + error.TargetSite + Environment.NewLine + error.StackTrace + error.InnerException);
            }
        }

        public static IRegistrationBuilder<TLimit, SimpleActivatorData, SingleRegistrationStyle> RegisterAdapterChain<TLimit>(ContainerBuilder builder, params Type[] types)
        {
            return
                builder
                .Register(c =>
                {
                    // http://stackoverflow.com/questions/23406641/how-to-mix-decorators-in-autofac
                    TLimit service = default(TLimit);
                    for (int index = 0; index < types.Length; ++index)
                    {
                        // resolve the keyed adapter, passing the previous service as the inner parameter
                        service = c.ResolveKeyed<TLimit>(types[index], TypedParameter.From(service));
                    }

                    return service;
                })
                .As<TLimit>();
        }
    }



}