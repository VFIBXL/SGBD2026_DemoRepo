using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Infrastructure
{
    public class BaseRepo
    {
        public string LoadFileFromAssembly(string fileName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var manifestResourceNames = assembly.GetManifestResourceNames();

            string resourceName = manifestResourceNames
                .FirstOrDefault(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                ?? throw new FileNotFoundException($"Embedded resource '{fileName}' not found.");

            using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
