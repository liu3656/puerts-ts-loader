using UnityEngine;
using System;
using System.Linq;

namespace Puerts
{
    public class TSLoader : Puerts.ILoader, Puerts.IModuleChecker
    {
        Puerts.DefaultLoader puerDefaultLoader;

        public TSLoader(string additionalBasePath): this(new string[]{ additionalBasePath }) {}

        public TSLoader(string[] additionalBasePath = null)
        {
            puerDefaultLoader = new Puerts.DefaultLoader();
#if UNITY_EDITOR
            if (additionalBasePath != null)
            {   
                foreach(string path in additionalBasePath)
                {
                    TSDirectoryCollector.AddTSCompiler(path);
                }
            }
#endif
        }

        public bool IsESM(string path)
        {
            return !path.EndsWith(".cjs") && !path.EndsWith(".cts");
        }

        public virtual string Resolve(string specifier)
        {
#if UNITY_EDITOR
            var fullPath = TSDirectoryCollector.TryGetFullTSPath(specifier);
            if (fullPath != null) return fullPath;
#else
            if (specifier.EndsWith(".ts") || specifier.EndsWith(".mts"))
            {
                specifier = specifier.Replace(".mts", ".mjs").Replace(".ts", ".js");
            }
#endif

            return puerDefaultLoader.FileExists(specifier) ? specifier : null;
        }

        public bool FileExists(string filename)
        {
            var resolveResult = Resolve(filename);
            return !string.IsNullOrEmpty(resolveResult);
        }
  
        public virtual string ReadFile(string specifier, out string debugpath)
        {
#if UNITY_EDITOR
            string filepath = Resolve(specifier);
            if (filepath.EndsWith("ts")) {
                debugpath = ""; 
                var content = TSDirectoryCollector.EmitTSFile(filepath);
                return content; 
            } 
#endif
            return puerDefaultLoader.ReadFile(specifier, out debugpath);
        }
    }
}