// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Topshelf
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Internal;
    using Internal.Actions;
    using log4net;

    /// <summary>
    /// Entry point into the Host infrastructure
    /// </summary>
    public static class Runner
    {
        static readonly IDictionary<NamedAction, IAction> _actions = new Dictionary<NamedAction, IAction>();
        static readonly ILog _log = LogManager.GetLogger(typeof (Runner));

        static Runner()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            _actions.Add(ServiceNamedAction.Install, new InstallServiceAction());
            _actions.Add(ServiceNamedAction.Uninstall, new UninstallServiceAction());
            _actions.Add(NamedAction.Console, new RunAsConsoleAction());
            _actions.Add(NamedAction.Gui, new RunAsWinFormAction());
            _actions.Add(ServiceNamedAction.Service, new RunAsServiceAction());
        }

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Fatal("Host encountered an unhandled exception on the AppDomain", (Exception) e.ExceptionObject);
        }

        /// <summary>
        /// Go go gadget
        /// </summary>
        public static void Host(IRunConfiguration configuration, string args)
        {
            _log.Info("Starting Host");
            _log.DebugFormat("Arguments: {0}", args);

            //make it so this can be passed in
            var a = TopshelfArgumentParser.Parse(Environment.CommandLine);
            TopshelfDispatcher.Dispatch(configuration, a);
        }
    }
}