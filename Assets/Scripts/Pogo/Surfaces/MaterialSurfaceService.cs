using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Surfaces
{
    public class MaterialSurfaceService
    {
        public SortedList<int, IMaterialSurfaceSource> Sources;
        private SurfaceConfig DefaultSurface;

        public MaterialSurfaceService(SurfaceConfig defaultSurface)
        {
            DefaultSurface = defaultSurface;
            Sources = new SortedList<int, IMaterialSurfaceSource>();
        }

        public SurfaceConfig CheckMaterial(Material material)
        {
            foreach(var source in Sources.Values)
            {
                SurfaceConfig result = source.CheckMaterial(material);
                if (result != null) return result;
            }
            return DefaultSurface;
        }

        /// <summary>
        /// Add a new Material Surface Source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="priority">order of evaluation of sources. lower numbers are evaluated first</param>
        /// <returns></returns>
        public MaterialSurfaceService AddSource(IMaterialSurfaceSource source, int priority)
        {
            Sources.Add(priority, source);
            return this;
        }

        public void RemoveSource(IMaterialSurfaceSource source)
        {
            int index = Sources.IndexOfValue(source);
            Sources.RemoveAt(index);
        }
    }
}
