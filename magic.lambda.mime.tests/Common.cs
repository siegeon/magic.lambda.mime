/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using magic.node;
using magic.node.services;
using magic.node.contracts;
using magic.signals.services;
using magic.signals.contracts;
using magic.node.extensions.hyperlambda;

namespace magic.lambda.mime.tests
{
    public static class Common
    {
        private class RootResolver : IRootResolver
        {
            public string DynamicFiles => AppDomain.CurrentDomain.BaseDirectory;
            public string RootFolder => AppDomain.CurrentDomain.BaseDirectory;

            public string AbsolutePath(string path)
            {
                return DynamicFiles + path.TrimStart(new char[] { '/', '\\' });
            }

            public string RelativePath(string path)
            {
                return path.Substring(DynamicFiles.Length - 1);
            }
        }

        static public Node Evaluate(string hl)
        {
            var services = Initialize();
            var lambda = HyperlambdaParser.Parse(hl);
            var signaler = services.GetService(typeof(ISignaler)) as ISignaler;
            signaler.Signal("eval", lambda);
            return lambda;
        }

        static public ISignaler GetSignaler()
        {
            var services = Initialize();
            return services.GetService(typeof(ISignaler)) as ISignaler;
        }

        #region [ -- Private helper methods -- ]

        static IServiceProvider Initialize()
        {
            var services = new ServiceCollection();
            services.AddTransient<ISignaler, Signaler>();
            services.AddTransient<IRootResolver, RootResolver>();
            services.AddTransient<IStreamService, StreamService>();
            var types = new SignalsProvider(InstantiateAllTypes<ISlot>(services));
            services.AddTransient<ISignalsProvider>((svc) => types);
            var provider = services.BuildServiceProvider();
            return provider;
        }

        static IEnumerable<Type> InstantiateAllTypes<T>(ServiceCollection services) where T : class
        {
            var type = typeof(T);
            var result = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !x.FullName.StartsWith("Microsoft", StringComparison.InvariantCulture))
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (var idx in result)
            {
                services.AddTransient(idx);
            }
            return result;
        }

        #endregion
    }
}
