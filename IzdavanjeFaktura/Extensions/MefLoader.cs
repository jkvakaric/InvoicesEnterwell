using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace IzdavanjeFaktura.Extensions
{
    public static class MefLoader
    {
        public static void Compose(object environment)
        {
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(catalog);
            container.ComposeParts(environment);
        }
    }
}