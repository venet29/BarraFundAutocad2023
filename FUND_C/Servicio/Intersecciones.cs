using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;


namespace FUND_C.Servicio
{
    public enum CasoInterssion
    {
        Hay1Interseccion,
        HayVAriasInterseccon,
        SinInterseccion,
        ConError
    }

    public class IntersecionDTo
    {
        public Point3d puntoIntersecion { get; set; }
        public double distanciaDesdeP1 { get; set; } // p1 en teoria en el orige, pq se busca la interseccion mas pequeña desde p1 hacia p2
        public ObjectId OBjetoIntersectadoID { get; internal set; }
        public Point3d P1_elementoIntersectado { get; set; }
        public Point3d P2_elementoIntersectado { get; set; }
        public Point3d PuntoIntersecion_menosRecub { get; internal set; }
    }
    public class Intersecciones
    {
        private Point3d P2_extendido;

        public Point3d P1 { get; set; }
        public Point3d P2 { get; set; }
        public Polyline acPoly { get; set; }
        public Point3d P2_interseccion { get; set; }
        public Point3d P2_interseccion_menosRecub { get; set; } // distacia menos recubrimiento
        public int LArgoExtension { get; set; }

        public CasoInterssion CosoInterseccion { get; set; }
        public Entity ent { get; private set; }

        private Document acDoc;
        private Editor acDocEd;
        private Database acCurDb;

        public List<IntersecionDTo> ListaInterseciones { get; set; }
        // punto esPunto Busqueda
        public Intersecciones(Point3d p1, Point3d p2, Polyline acPoly)
        {
            P1 = p1;
            P2 = p2;
            this.acPoly = acPoly;
            LArgoExtension = 10;
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            acCurDb = acDoc.Database;
         
            CosoInterseccion = CasoInterssion.SinInterseccion;
            ListaInterseciones = new List<IntersecionDTo>();
        }



        public bool EjecutarConCurve(double cuantoEntender, double recubrimiento = 2.0d)
        {
            try
            {
                Point3d direccion_p2_p1 = Util.NormalizeDifference(P1, P2);
                P2_extendido = P2.Add(direccion_p2_p1.GetAsVector() * LArgoExtension);
                // var dire = p2 - p3;
                Line cur = new Line(P1, P2);

                Point3d[] values_sup = new Point3d[2];
                values_sup[0] = P1;
                values_sup[1] = P2_extendido;

                TypedValue[] acTypValAr = new TypedValue[5]
                {
                        new TypedValue((int)DxfCode.Operator, "<or"),
                        new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
                        new TypedValue((int)DxfCode.Start, "LINE"),
                        new TypedValue((int)DxfCode.Operator, "or>"),
                        new TypedValue((int)DxfCode.LayerName, "FUNDACIONES")
                };

                // Iniciar una transacción
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
               
          
                    SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
                    PromptSelectionResult acSSPrompt2 = acDocEd.SelectCrossingWindow(values_sup[0], values_sup[1], acSelFtr);

                    Point3d[] values_inter = new Point3d[4];
                    int i = 0;

                    // Supongo que FUNDACIONES es una clase externa que tienes
                    FUNDACIONES fundacion_ = new FUNDACIONES();
                    //Point3d _pto_barra;

                    if (acSSPrompt2.Status == PromptStatus.OK)
                    {
                        SelectionSet acSSet = acSSPrompt2.Value;

                        // Recorrer los objetos del conjunto de selección
                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            if (acSSObj != null)
                            {
                                Entity excur = (Entity)acTrans.GetObject(acSSObj.ObjectId, OpenMode.ForRead);

                                if (excur != null)
                                {
                                    Point3dCollection pts = new Point3dCollection();
                                    cur.IntersectWith(excur, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                                    string[] datos = new string[2];
                                    for (int k = 0; k < pts.Count; k++)
                                    {
                                        Point3d resultPuntoStar = default;
                                        Point3d resultPuntoEnd = default;
                                        if (acSSObj.ObjectId.ObjectClass.DxfName.ToString() == "LWPOLYLINE")
                                        {
                                            Line segment = new Line(P1, pts[k]);
                                            (bool result, Point3d resultPuntoStar1, Point3d resultPuntoEnd1) = CheckPolylineIntersection((Polyline)excur, segment);
                                            resultPuntoStar = resultPuntoStar1;
                                            resultPuntoEnd = resultPuntoEnd1;
                                        }
                                        else if (acSSObj.ObjectId.ObjectClass.DxfName.ToString() == "LINE")
                                        {
                                            Line lineaIntersectadao = (Line)excur;
                                            resultPuntoStar = lineaIntersectadao.StartPoint;
                                            resultPuntoEnd = lineaIntersectadao.EndPoint;
                                        }


                                        var newInter = new IntersecionDTo()
                                        {
                                            puntoIntersecion = pts[k],
                                            distanciaDesdeP1 = P1.DistanceTo(pts[k]),
                                            OBjetoIntersectadoID = acSSObj.ObjectId,
                                            P1_elementoIntersectado = resultPuntoStar,
                                            P2_elementoIntersectado = resultPuntoEnd
                                        };

                                        if (ListaInterseciones.Exists(c => Math.Abs(c.distanciaDesdeP1 - newInter.distanciaDesdeP1) < 0.1))
                                            continue;
                                        ListaInterseciones.Add(newInter);
                                    }
                                }
                            }
                        }
                    }

                    acTrans.Abort();
                }

                ListaInterseciones = ListaInterseciones.OrderBy(c => c.distanciaDesdeP1).ToList();

                if (ListaInterseciones.Count == 0)
                {
                    CosoInterseccion = CasoInterssion.SinInterseccion;
                    return false;
                }
                else if (ListaInterseciones.Count == 1)
                {
                    CosoInterseccion = CasoInterssion.Hay1Interseccion;
                }
                else
                    CosoInterseccion = CasoInterssion.HayVAriasInterseccon;


                P2_interseccion = ListaInterseciones[0].puntoIntersecion;
                P2_interseccion_menosRecub = P2_interseccion.Add(-direccion_p2_p1.GetAsVector() * recubrimiento);
                ListaInterseciones[0].PuntoIntersecion_menosRecub = P2_interseccion_menosRecub;
            }
            catch (Exception ex)
            {
                Util.ErrorMsg($"Error en 'function'. ex:{ex.Message}");
                CosoInterseccion = CasoInterssion.ConError;
                return false;
            }
            return true;
        }


        public (bool, Point3d, Point3d) CheckPolylineIntersection(Polyline pline, Line selectedLine)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            try
            {
                //using (Transaction tr = db.TransactionManager.StartTransaction())
                //{
                int numberOfVertices = pline.NumberOfVertices;
                for (int i = 0; i < numberOfVertices; i++)
                {
                    Point2d start = pline.GetPoint2dAt(i);
                    Point2d end = pline.GetPoint2dAt((i + 1) % numberOfVertices); // To loop back to the first point for the last segment

                    Line segment = new Line(new Point3d(start.X, start.Y, 0), new Point3d(end.X, end.Y, 0));

                    (bool result, Point3d puntoInter) = IsIntersecting(selectedLine, segment);
                    if (result)
                    {
                        return (true, new Point3d(start.X, start.Y, 0), new Point3d(end.X, end.Y, 0));
                    }
                }

                //    tr.Commit();
                //}
            }
            catch (Exception)
            {
            }
            return (false, Point3d.Origin, Point3d.Origin);
        }

        public (bool, Point3d) IsIntersecting(Line line1, Line line2)
        {
            Point3dCollection intersectionPoints = new Point3dCollection();
            line1.IntersectWith(line2, Intersect.OnBothOperands, intersectionPoints, IntPtr.Zero, IntPtr.Zero);
            return (intersectionPoints.Count > 0 ? (true, intersectionPoints[0]) : (false, Point3d.Origin));
        }
    }
}
