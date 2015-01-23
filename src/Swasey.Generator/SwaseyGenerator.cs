﻿using System;
using System.IO;
using System.Linq;

using Handlebars;

using Swasey.Helpers;

namespace Swasey
{
    public static class SwaseyGenerator
    {

        static SwaseyGenerator()
        {
            Engine = new Lazy<IHandlebars>(() => Handlebars.Handlebars.Create());

            // Initialize built-in handlers
            RegisterHelper(new FileHeader(HelperTemplates.HelperTemplate_FileHeader.Compile()));
            RegisterHelper(new OperationParameters(HelperTemplates.HelperTemplate_OperationParameters.Compile()));
            RegisterHelper(new PascalCase());
        }

        internal static Lazy<IHandlebars> Engine { get; private set; }

        public static void RegisterHelper(IBlockHelper helper, string helperName = null)
        {
            helperName = helperName ?? helper.GetType().Name;
            Engine.Value.RegisterHelper(helperName, (tw, opts, ctx, args) => helper.Run(tw, opts, ctx, args));
        }

        public static void RegisterHelper(IInlineHelper helper, string helperName = null)
        {
            helperName = helperName ?? helper.GetType().Name;
            Engine.Value.RegisterHelper(helperName, (tw, ctx, args) => helper.Run(tw, ctx, args));
        }

        public static void RegisterTemplate(string name, string template)
        {
            using (var sr = new StringReader(template))
            {
                RegisterTemplate(name, sr);
            }
        }

        public static void RegisterTemplate(string name, TextReader template)
        {
            Engine.Value.RegisterTemplate(name, CompileTemplate(template));
        }

        public static Action<TextWriter, object> CompileTemplate(TextReader template)
        {
            return Engine.Value.Compile(template);
        }

        public static string RenderRawTemplate(string template, object context)
        {
            return Engine.Value.Compile(template)(context);
        }

        internal static Action<TextWriter, object> Compile(this Lazy<string> This)
        {
            using (var sr = new StringReader(This.Value))
            {
                return CompileTemplate(sr);
            }
        }

    }
}