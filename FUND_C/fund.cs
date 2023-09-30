using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUND_C
{
    public class fund
    {

        [CommandMethod("TEST")]
        public void Test()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var modelSpace = (BlockTableRecord)tr.GetObject(
                    SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite);

                // get all curve entities in model space
                var curveClass = RXObject.GetClass(typeof(Curve));
                var curves = modelSpace
                    .Cast<ObjectId>()
                    .Where(id => id.ObjectClass.IsDerivedFrom(curveClass))
                    .Select(id => (Curve)tr.GetObject(id, OpenMode.ForRead))
                    .ToArray();

                // get all intersections (brute force algorithm O(n²) complexity)
                var points = new Point3dCollection();
                for (int i = 0; i < curves.Length - 1; i++)
                {
                    for (int j = i + 1; j < curves.Length; j++)
                    {
                        curves[i].IntersectWith(curves[j], Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
                    }
                }

                // draw the circles
                double radius = (db.Extmax.Y - db.Extmin.Y) / 100.0;
                foreach (Point3d point in points)
                {
                    var circle = new Circle(point, Vector3d.ZAxis, radius);
                    circle.ColorIndex = 1;
                    modelSpace.AppendEntity(circle);
                    tr.AddNewlyCreatedDBObject(circle, true);
                }
                tr.Commit();
            }
        }
    }
}
