﻿using System.Web.UI;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.Forms.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Forms.Mvc.Models.Fields;
using Telerik.Sitefinity.Modules.ControlTemplates;
using Telerik.Sitefinity.Modules.Forms.Configuration;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ContentUI;
using Telerik.Sitefinity.Web.UI.ContentUI.Views.Backend.Detail;

namespace Telerik.Sitefinity.Frontend.Forms
{
    public static class Initializer
    {
        public static void Initialize()
        {
            VirtualPathManager.AddVirtualFileResolver<FormsVirtualRazorResolver>(FormsVirtualRazorResolver.Path + "*", "MvcFormsResolver");
            ObjectFactory.Container.RegisterInstance<IControlDefinitionExtender>("FormsDefinitionsExtender", new FormsDefinitionsExtender(), new ContainerControlledLifetimeManager());

            ObjectFactory.Container.RegisterType<IFormFieldBackendConfigurator, BackendFieldFallbackConfigurator>(typeof(MvcControllerProxy).FullName);

            EventHub.Unsubscribe<IScriptsRegisteringEvent>(Initializer.RegisteringFormScriptsHandler);
            EventHub.Subscribe<IScriptsRegisteringEvent>(Initializer.RegisteringFormScriptsHandler);

            Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        private static void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                Initializer.UnregisterTemplatableControl();
            }
        }

        private static void UnregisterTemplatableControl()
        {
            ControlTemplates.UnregisterTemplatableControl(new ControlTemplateInfo() { ControlType = typeof(FormController), AreaName = "Form" });
        }

        private static void RegisteringFormScriptsHandler(IScriptsRegisteringEvent @event)
        {
            var zoneEditor = @event.Sender as ZoneEditor;
            if (zoneEditor != null && zoneEditor.MediaType == DesignMediaType.Form)
            {
                @event.Scripts.Add(new ScriptReference(string.Format("~/Frontend-Assembly/{0}/Mvc/Scripts/Form/form.js", typeof(Initializer).Assembly.GetName().Name)));
            }
        }
    }
}