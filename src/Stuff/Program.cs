﻿// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Stuff
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Timers;
    using log4net.Config;
    using Microsoft.Practices.ServiceLocation;
    using StructureMap;
    using Topshelf;
    using Topshelf.Configuration;

    internal class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(".\\log4net.config"));
            IRunConfiguration cfg = RunnerConfigurator.New(x =>
            {
                x.AfterStoppingTheHost(h => { Console.WriteLine("AfterStop called invoked, services are stopping"); });

                x.ConfigureServiceInIsolation<TownCrier>("tc", s =>
                {
                    s.CreateServiceLocator(()=>
                    {
                        ObjectFactory.Initialize(i =>
                        {
                            i.ForConcreteType<TownCrier>().Configure.WithName("tc");
                        });

                        return new StructureMapServiceLocator();
                    });
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
//                x.ConfigureService<TownCrier>("tc", s =>
//                {
//                    s.CreateServiceLocator(() =>
//                    {
//                        ObjectFactory.Initialize(i =>
//                        {
//                            i.ForConcreteType<TownCrier>().Configure.WithName("tc");
//                            i.ForConcreteType<ServiceConsole>(); //bah why do I have to register this?
//                        });
//                        return new StructureMapServiceLocator();
//                    });
//                    s.WhenStarted(tc => tc.Start());
//                    s.WhenStopped(tc => tc.Stop());
//                });

                x.RunAsLocalSystem();

                x.SetDescription("Sample Topshelf Host");
                x.SetDisplayName("Stuff");
                x.SetServiceName("stuff");
            });

            Runner.Host(cfg, Environment.CommandLine);
        }
    }

    public class TownCrier
    {
        readonly Timer _timer;

        public TownCrier()
        {
            _timer = new Timer(1000) {AutoReset = true};
            _timer.Elapsed += (sender, eventArgs) => Console.WriteLine(DateTime.Now);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }

    public class StructureMapServiceLocator :
        IServiceLocator
    {
        #region IServiceLocator Members

        public object GetService(Type serviceType)
        {
            return ObjectFactory.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType)
        {
            return ObjectFactory.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return ObjectFactory.GetNamedInstance(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            foreach (object instance in ObjectFactory.GetAllInstances(serviceType))
            {
                yield return instance;
            }
        }

        public TService GetInstance<TService>()
        {
            return ObjectFactory.GetInstance<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return ObjectFactory.GetNamedInstance<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return ObjectFactory.GetAllInstances<TService>();
        }

        #endregion
    }
}