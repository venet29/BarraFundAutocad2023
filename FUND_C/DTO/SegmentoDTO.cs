using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUND_C.DTO
{
    public class SegmentoDTO
    {
        public Point3d start { get; set; }
        public Point3d end { get; set; }
        public Line Segmento { get; set; }
        public double Largo { get; private set; }
        public Point3d Direccion_p2_p1 { get; private set; }

        public SegmentoDTO(Point3d start, Point3d end)
        {
            this.start = start;
            this.end = end;
            Segmento = new Line(start, end);
            Largo = start.DistanceTo(end);
            Direccion_p2_p1 = Util.NormalizeDifference(start, end);
        }
    }
}
