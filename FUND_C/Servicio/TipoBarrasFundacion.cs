using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using FUND_C.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUND_C.Servicio
{
    public class TipoBarrasFundacion
    {
        public string TipoBarras { get; private set; }
        public string Ubicacion { get; private set; }

        public bool Buscar(ObjectId objectId)
        {
            try
            {
                Polyline pline = new Polyline();
                BarraFundacionesDTO _BarraFundacionesNew = new BarraFundacionesDTO();
                int numberOfVertices = pline.NumberOfVertices;

                _BarraFundacionesNew.NumberOfVertices = numberOfVertices;
                for (int i = 0; i < numberOfVertices; i++)
                {
                    Point2d start = pline.GetPoint2dAt(i);
                    Point2d end = pline.GetPoint2dAt((i + 1) % numberOfVertices); // To loop back to the first point for the last segment

                    //Line segment = new Line(new Point3d(start.X, start.Y, 0), new Point3d(end.X, end.Y, 0));
                    _BarraFundacionesNew.ListaSegmento.Add(new SegmentoDTO(new Point3d(start.X, start.Y, 0), new Point3d(end.X, end.Y, 0)));
                }

                if (_BarraFundacionesNew.ListaSegmento.Count == 3)
                {
                    var segmentoExtremo = _BarraFundacionesNew.ListaSegmento[0];

                    if (segmentoExtremo.Direccion_p2_p1.Y == 0) // vertica
                    {
                        if (segmentoExtremo.Direccion_p2_p1.X < 0)
                        {
                            TipoBarras = "f9v";
                            Ubicacion = "inferior";
                        }
                        else
                        {
                            TipoBarras = "f11";
                            Ubicacion = "superior";
                        }
                    }
                    else // horizontal o inclianda
                    {

                        if (segmentoExtremo.Direccion_p2_p1.Y > 0)
                        {
                            TipoBarras = "f9v";
                            Ubicacion = "inferior";
                        }
                        else
                        {
                            TipoBarras = "f11";
                            Ubicacion = "superior";
                        }
                    }
                }
                else if (_BarraFundacionesNew.ListaSegmento.Count == 2)
                {
                    var pata = _BarraFundacionesNew.ListaSegmento.OrderBy(c=> c.Largo).FirstOrDefault();
                    var Largo = _BarraFundacionesNew.ListaSegmento.OrderBy(c => c.Largo).Last();

                    if (pata.Direccion_p2_p1.Y == 0) // vertica
                    {
                        if (pata.Direccion_p2_p1.X < 0)
                        {
                            TipoBarras = "f9v";
                            Ubicacion = "inferior";
                        }
                        else
                        {
                            TipoBarras = "f11";
                            Ubicacion = "superior";
                        }
                    }
                    else // horizontal o inclianda
                    {

                        if (pata.Direccion_p2_p1.Y > 0)
                        {
                            TipoBarras = "f9v";
                            Ubicacion = "inferior";
                        }
                        else
                        {
                            TipoBarras = "f11";
                            Ubicacion = "superior";
                        }
                    }

                }
                else if (_BarraFundacionesNew.ListaSegmento.Count == 1)
                {
                    TipoBarras = "f3";
                    Ubicacion = "inferior";
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                Util.ErrorMsg($"Error en 'function'. ex:{ex.Message}");
                return false;
            }
            return true;
        }

    }
}
