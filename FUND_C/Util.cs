using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using WinForms = System.Windows.Forms;
using System.Diagnostics;
//using System.Windows.Forms;

namespace FUND_C
{
    public class Util
    {
        BlockReference acBlkRef = null;

        public static float Aproximar( float valor, float referen,  int diametro)
        {
            float result = 0;

            string auxString = valor.ToString();

            if (diametro <= 36)
            {
                string[] split = auxString.Split(new char[] { '.', '\t' });
                int parteEntera = Convert.ToInt32(split[0].Substring(split[0].Length - 1));

                if (split.Length == 2)
                {
                    valor = parteEntera + float.Parse("0." + split[1]);
                }
                else
                {
                    valor = parteEntera;
                }

                if (valor < 0.01)
                {
                    result = 0;
                }
                else if (valor < 5)
                {
                    result = 5 - valor;
                }
                else if (valor == 5)
                {
                    result = 0;
                }
                else if (valor < 5.00000001)
                {
                    result = 0;
                }
                else if (valor < 10)
                {
                    result = 10 - valor;
                }
                else
                {
                    result = 0;
                }
            }
            else if (diametro == 16 || diametro > 16)
            {
                valor = float.Parse(valor.ToString("F2").Substring(1));

                if (valor == 0)
                {
                    result = 0;
                }
                else if (valor <= 25)
                {
                    result = 25 - valor;
                }
                else if (valor <= 50)
                {
                    result = 50 - valor;
                }
                else if (valor <= 75)
                    result = 75 - valor;
                else if (valor <= 100)
                    result = 100 - valor;
            }
            else if (diametro > 16)
            {
                valor = float.Parse(valor.ToString("F2").Substring(1));

                if (valor <= 50)
                    result = 50 - valor;
                else if (valor <= 100)
                    result = 100 - valor;
            }

            if (referen != 0 && referen < 0)
            {
                result = -result;
            }

            return result;
        }

        public static double Aproximar2Fund( double valor,  float referen)
        {
            double result = 0;

            string[] split = valor.ToString().Split(new char[] { '.', '\t' });

            if (split.Length == 2)
                valor = float.Parse("0." + split[1]);
            else
                valor = 0;

            if (valor < 0.01)
                result = 0;
            else if (valor < 1)
                result = 1 - valor;
            else
                result = 0;

            if (referen != 0 && referen < 0)
                result = -result;

            return result;
        }

        public static double CoordenadaAnguloP1P2Fun(Point3d pt1, Point3d pt2, Editor ed, float angle)
        {
            double ang;

            if (ed == null)
            {
                ang = angle;
            }
            else
            {
                Matrix3d ucsmtx = ed.CurrentUserCoordinateSystem;
                CoordinateSystem3d ucs = ucsmtx.CoordinateSystem3d;
                Plane ucsplane = new Plane(ucs.Origin, ucs.Xaxis, ucs.Yaxis);

                Vector3d vec = pt2 - pt1;
                ang = vec.AngleOnPlane(ucsplane);

                if (ang > Math.PI * 6) AdjustAngle(ref ang, Math.PI * 6);
                else if (ang > Math.PI * 5) AdjustAngle(ref ang, Math.PI * 5);
                else if (ang > Math.PI * 4) AdjustAngle(ref ang, Math.PI * 4);
                else if (ang > Math.PI * 3) AdjustAngle(ref ang, Math.PI * 3);
                else if (ang > Math.PI * 2) AdjustAngle(ref ang, Math.PI * 2);
                else if (ang > Math.PI) AdjustAngle(ref ang, Math.PI);
                else if (ang > Math.PI / 2 * 1.02) ang -= Math.PI;
            }

            return ang;
        }

        private static void AdjustAngle(ref double ang, double value)
        {
            ang -= value;
            if (ang > Math.PI / 2 * 1.02)
            {
                ang -= Math.PI;
            }
        }

        public static Point3d[] CoordenadaModificarFun(Entity entidad, Point3d pt1, Point3d pt2)
        {
            Point3d[] result = new Point3d[2];

            if (entidad != null && entidad.ObjectId.ObjectClass.DxfName.ToString() == "LINE")
            {
                // Placeholder for LINE specific logic if needed in the future.
            }

            if (Math.Abs(pt1.X - pt2.X) < 0.1)
            {
                if (pt2.Y < pt1.Y)
                {
                    result[0] = pt2;
                    result[1] = pt1;
                }
                else
                {
                    result[0] = pt1;
                    result[1] = pt2;
                }
            }
            else
            {
                if (pt2.X < pt1.X)
                {
                    result[0] = pt2;
                    result[1] = pt1;
                }
                else
                {
                    result[0] = pt1;
                    result[1] = pt2;
                }
            }
            return result;
        }


        public static void ErrorMsg(string msg, bool IsRegistrar = true)
        {
            Debug.WriteLine(msg);
            WinForms.MessageBox.Show(msg,
              "Mensaje",
              WinForms.MessageBoxButtons.OK,
              WinForms.MessageBoxIcon.Error);
        }
        public static void ZoomFund(Point3d pMin, Point3d pMax, Point3d pCenter, double dFactor)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            int nCurVport = System.Convert.ToInt32(Application.GetSystemVariable("CVPORT"));

            if (acCurDb.TileMode)
            {
                if (pMin == Point3d.Origin && pMax == Point3d.Origin)
                {
                    pMin = acCurDb.Extmin;
                    pMax = acCurDb.Extmax;
                }
            }
            else
            {
                if (nCurVport == 1)
                {
                    if (pMin == Point3d.Origin && pMax == Point3d.Origin)
                    {
                        pMin = acCurDb.Pextmin;
                        pMax = acCurDb.Pextmax;
                    }
                }
                else
                {
                    if (pMin == Point3d.Origin && pMax == Point3d.Origin)
                    {
                        pMin = acCurDb.Extmin;
                        pMax = acCurDb.Extmax;
                    }
                }
            }

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (ViewTableRecord acView = acDoc.Editor.GetCurrentView())
                {
                    Matrix3d matWCS2DCS = Matrix3d.PlaneToWorld(acView.ViewDirection);
                    matWCS2DCS = Matrix3d.Displacement(acView.Target - Point3d.Origin) * matWCS2DCS;
                    matWCS2DCS = Matrix3d.Rotation(-acView.ViewTwist, acView.ViewDirection, acView.Target) * matWCS2DCS;

                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        pMin = new Point3d(pCenter.X - (acView.Width / 2), pCenter.Y - (acView.Height / 2), 0);
                        pMax = new Point3d((acView.Width / 2) + pCenter.X, (acView.Height / 2) + pCenter.Y, 0);
                    }

                    Extents3d eExtents;
                    using (Line acLine = new Line(pMin, pMax))
                    {
                        eExtents = new Extents3d(acLine.Bounds.Value.MinPoint, acLine.Bounds.Value.MaxPoint);
                    }

                    double dViewRatio = (acView.Width / acView.Height);
                    matWCS2DCS = matWCS2DCS.Inverse();
                    eExtents.TransformBy(matWCS2DCS);

                    double dWidth, dHeight;
                    Point2d pNewCentPt;

                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        dWidth = acView.Width;
                        dHeight = acView.Height;

                        if (dFactor == 0)
                        {
                            pCenter = pCenter.TransformBy(matWCS2DCS);
                        }

                        pNewCentPt = new Point2d(pCenter.X, pCenter.Y);
                    }
                    else
                    {
                        dWidth = eExtents.MaxPoint.X - eExtents.MinPoint.X;
                        dHeight = eExtents.MaxPoint.Y - eExtents.MinPoint.Y;
                        pNewCentPt = new Point2d(((eExtents.MaxPoint.X + eExtents.MinPoint.X) * 0.5), ((eExtents.MaxPoint.Y + eExtents.MinPoint.Y) * 0.5));
                    }

                    if (dWidth > (dHeight * dViewRatio))
                    {
                        dHeight = dWidth / dViewRatio;
                    }

                    if (dFactor != 0)
                    {
                        acView.Height = dHeight * dFactor;
                        acView.Width = dWidth * dFactor;
                    }

                    acView.CenterPoint = pNewCentPt;
                    acDoc.Editor.SetCurrentView(acView);
                }

                acTrans.Commit();
            }
        }

        public Point3d NormalizeVector(Point3d inputVector)
        {
            // Calcular la magnitud del vector
            double magnitude = Math.Sqrt(Math.Pow(inputVector.X, 2) + Math.Pow(inputVector.Y, 2));

            // Si la magnitud es cero, no es posible normalizar
            if (magnitude == 0)
            {
                throw new InvalidOperationException("El vector no puede tener magnitud cero.");
            }

            // Devolver el vector normalizado
            return new Point3d(inputVector.X / magnitude, inputVector.Y / magnitude, 0);
        }

        //diferencia p2 -p1
        public static Point3d NormalizeDifference(Point3d P1, Point3d P2)
        {
            // Calcula el vector diferencia
            Point3d diffVector = new Point3d(P2.X - P1.X, P2.Y - P1.Y,0);

            // Calcular la magnitud del vector diferencia
            double magnitude = Math.Sqrt(Math.Pow(diffVector.X, 2) + Math.Pow(diffVector.Y, 2));

            // Si la magnitud es cero, no es posible normalizar
            if (magnitude == 0)
            {
                throw new InvalidOperationException("El vector resultante no puede tener magnitud cero.");
            }

            // Devolver el vector normalizado
            return new Point3d(diffVector.X / magnitude, diffVector.Y / magnitude,0);
        }

        //columnabuscar=0  busca en la primera colmna 
        //columnaResultado=1 devuleve resultado de la columna 1
        public static string ObtenerValorDataGrid(System.Windows.Forms.DataGridView dataGridView, string searchValue, int columnabuscar, int columnaResultado)
        {
            foreach (System.Windows.Forms.DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[columnabuscar].Value != null && row.Cells[columnabuscar].Value.ToString() == searchValue)
                {
                    return row.Cells[columnaResultado].Value.ToString();
                }
            }
            return string.Empty; // Valor no encontrado
        }
    }
}
