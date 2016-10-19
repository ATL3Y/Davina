using System;
using System.Collections.Generic;

namespace Scripts.HairTool.Geometry.Create
{
    [Serializable]
    public class CreatorGeometry
    {
        public List<GeometryGroupData> List = new List<GeometryGroupData>(); 
        public int SelectedIndex;

        public GeometryGroupData Selected
        {
            get { return SelectedIndex >= 0 && SelectedIndex < List.Count
                    ? List[SelectedIndex] 
                    : null;
            }
        }
    }
}
