using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using VARIOS;
using Autodesk.AutoCAD.Customization;

namespace FUND_C
{

    public class FUNDACIONES
    {
        private Point2d punto_ini;
        private Point2d punto_fin;
        private Point2d punto_dim1;
        private Point2d punto_dim2;
        private float punto_angle = 0;
        private float planta_desfase0_2 = 0;
        private float planta_desfase0 = 0;
        private float planta_desfase1 = 0;
        private float planta_desfase2 = 0;
        private float planta_aux1 = 0;
        private float planta_aux2 = 0;
        private float punto_e = 0;
        private float punto_e_real = 0;
        private float largo_min_losa = 0;
        private float largo_barra_fund = 0;
        private string largo_barra_parcial = "0";
        private float espesor_muro1 = 0;
        private float espesor_muro2 = 0;
        private string tipo_losa;
        private string tipo_losa2;
        private string tipo_orientacion;
        private int diamtro_;
        private ObjectId ref_obj;
        private ObjectId ref_obj2 = default;
        private string punto_cua_e;
        private Point2d punto_uso;

        public Point2d Punto_uso_
        {
            get { return punto_uso; }
            set { punto_uso = value; }
        }

        public string Orientacion_
        {
            get { return tipo_orientacion; }
            set { tipo_orientacion = value; }
        }

        public string Tipo
        {
            get { return tipo_losa; }
            set { tipo_losa = value; }
        }

        public string Tipo2
        {
            get { return tipo_losa2; }
            set { tipo_losa2 = value; }
        }

        public int Diamet
        {
            get { return diamtro_; }
            set { diamtro_ = value; }
        }

        public float Largo_barra_real_fun
        {
            get { return largo_barra_fund; }
            set { largo_barra_fund = value; }
        }

        public string Largo_barra_parcial_fun
        {
            get { return largo_barra_parcial; }
            set { largo_barra_parcial = value; }
        }

        public Point2d Punto_inicial
        {
            get { return punto_ini; }
            set { punto_ini = value; }
        }

        public float E_muro1
        {
            get { return espesor_muro1; }
            set { espesor_muro1 = value; }
        }

        public float E_muro2
        {
            get { return espesor_muro2; }
            set { espesor_muro2 = value; }
        }

        public float Largo_minimo_losa
        {
            get { return largo_min_losa; }
            set { largo_min_losa = value; }
        }

        public ObjectId Object_poly
        {
            get { return ref_obj; }
            set { ref_obj = value; }
        }

        public ObjectId Object_poly2
        {
            get { return ref_obj2; }
            set { ref_obj2 = value; }
        }

        public float Punto_e_losa
        {
            get { return punto_e; }
            set { punto_e = value; }
        }

        public float Punto_e_losa_real
        {
            get { return punto_e_real; }
            set { punto_e_real = value; }
        }

        public string Punto_cua_losa
        {
            get { return punto_cua_e; }
            set { punto_cua_e = value; }
        }

        public Point2d Punto_final
        {
            get { return punto_fin; }
            set { punto_fin = value; }
        }

        public Point2d Punto_dimension1
        {
            get { return punto_dim1; }
            set { punto_dim1 = value; }
        }

        public Point2d Punto_dimension2
        {
            get { return punto_dim2; }
            set { punto_dim2 = value; }
        }

        public float Punto_angulo
        {
            get { return punto_angle; }
            set { punto_angle = value; }
        }

        public float Punto_DESFASE0_2
        {
            get { return planta_desfase0_2; }
            set { planta_desfase0_2 = value; }
        }

        public float Punto_DESFASE0
        {
            get { return planta_desfase0; }
            set { planta_desfase0 = value; }
        }

        public float Punto_DESFASE1
        {
            get { return planta_desfase1; }
            set { planta_desfase1 = value; }
        }

        public float Punto_DESFASE2
        {
            get { return planta_desfase2; }
            set { planta_desfase2 = value; }
        }

        public float Punto_aux1
        {
            get { return planta_aux1; }
            set { planta_aux1 = value; }
        }

        public float Punto_aux2
        {
            get { return planta_aux2; }
            set { planta_aux2 = value; }
        }

        public FUNDACIONES()
        {

        }
        public void modificar_barra_FUNDA(ref ObjectIdCollection ents, ref Entity ent, Point2d pt_ini, Point2d pt_fin, string casos, ref string direcci, int diamtro_pl, float e1)
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            Editor Ed = Doc.Editor;

            using (Transaction acTrans = DB.TransactionManager.StartTransaction())
            {
                ObjectId acPolyObj = ObjectId.Null;
                Polyline acEnt_barra_aux = acTrans.GetObject(ent.ObjectId, OpenMode.ForWrite) as Polyline;

                Point3d pts1;
                Point3d pts2;
                Point2d pts1_v;
                Point2d pts2_v;
                Point3d[] coordenada_PTO_ = new Point3d[2];

                Polyline acPoly = new Polyline();
                coordenada_PTO_ = Util.CoordenadaModificarFun(null, new Point3d(pt_ini.X, pt_ini.Y, 0), new Point3d(pt_fin.X, pt_fin.Y, 0)); // OBTENER P1 Y P2 ORDENADOS
                pts1 = coordenada_PTO_[0];
                pts2 = coordenada_PTO_[1];
                double ANGLE = Util.CoordenadaAnguloP1P2Fun(pts1, pts2, Ed, 0);
                float desplaza = 2f;

                coordenada_PTO_ = Util.CoordenadaModificarFun(null, new Point3d(pt_ini.X, pt_ini.Y, 0), new Point3d(pt_fin.X, pt_fin.Y, 0));

                pts2_v = new Point2d(coordenada_PTO_[1].X, coordenada_PTO_[1].Y);
                pts1_v = new Point2d(coordenada_PTO_[0].X, coordenada_PTO_[0].Y);
                Point2d p0, p1, p2, p3, p4, p5, p6;
                p0 = p1 = p2 = p3 = p4 = p5 = p6 = new Point2d();
                float delta0, delta0_2, delta1, delta2;
                delta0 = delta0_2 = delta1 = delta2 = 0f;

                punto_ini = pts1_v;
                punto_fin = pts2_v;
                punto_angle = (float)ANGLE;

                // Definimos los puntos y variables que se usan en los condicionales.

                float delta_aux2 = 0, delta_aux1 = 0;

                // Comprobamos los diferentes casos
                if (casos == "f3")
                {
                    p0 = new Point2d(pts1_v.X, pts1_v.Y);
                    p1 = new Point2d(pts2_v.X, pts2_v.Y);
                    delta1 = Util.Aproximar((float)p0.GetDistanceTo(p1), 1, (int)diamtro_pl);

                    acEnt_barra_aux.SetPointAt(0, new Point2d(pts1_v.X - Math.Cos(ANGLE) * delta1 / 2, pts1_v.Y - Math.Sin(ANGLE) * delta1 / 2));
                    acEnt_barra_aux.SetPointAt(1, new Point2d(pts2_v.X + Math.Cos(ANGLE) * delta1 / 2, pts2_v.Y + Math.Sin(ANGLE) * delta1 / 2));

                    largo_barra_fund = (float)Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0);
                    largo_barra_parcial = "";
                    Punto_uso_ = acEnt_barra_aux.GetPoint2dAt(1);
                }
                else if (casos == "f9a")
                {
                    p0 = new Point2d(pts1_v.X + Math.Sin(ANGLE) * e1, pts1_v.Y - Math.Cos(ANGLE) * e1);
                    p1 = new Point2d(pts1_v.X, pts1_v.Y);
                    p2 = new Point2d(pts2_v.X, pts2_v.Y);
                    p3 = new Point2d(pts2_v.X + Math.Sin(ANGLE) * e1, pts2_v.Y - Math.Cos(ANGLE) * e1);

                    delta0 = (float)Util.Aproximar2Fund(p1.GetDistanceTo(p2), 1);
                    delta1 = Util.Aproximar((float)(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + p2.GetDistanceTo(p3) + delta0), 1, diamtro_pl);

                    if (delta1 % 2 != 0)
                    {
                        delta_aux1 = delta1 / 2 - 0.5f;
                        delta_aux2 = delta1 / 2 + 0.5f;
                    }
                    else
                    {
                        delta_aux1 = delta1 / 2;
                        delta_aux2 = delta1 / 2;
                    }

                    delta1 = 0;

                    acEnt_barra_aux.SetPointAt(0, new Point2d(pts1_v.X - Math.Cos(ANGLE) * (delta0) + Math.Sin(ANGLE) * (e1 + delta_aux1), pts1_v.Y - Math.Sin(ANGLE) * (delta0) - Math.Cos(ANGLE) * (e1 + delta_aux1)));
                    acEnt_barra_aux.SetPointAt(1, new Point2d(pts1_v.X - Math.Cos(ANGLE) * delta0, pts1_v.Y - Math.Sin(ANGLE) * delta0));
                    acEnt_barra_aux.AddVertexAt(2, new Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0);
                    acEnt_barra_aux.AddVertexAt(3, new Point2d(pts2_v.X + Math.Sin(ANGLE) * (e1 + delta_aux2), pts2_v.Y - Math.Cos(ANGLE) * (e1 + delta_aux2)), 0, 0, 0);

                    largo_barra_fund = (float)Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)) + acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0);
                    largo_barra_parcial = "(" + Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) + "+" + Math.Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) + "+" + Math.Round(acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0) + ")";
                    Punto_aux1 = delta_aux1;
                    Punto_aux2 = delta_aux2;
                }
                else if (casos == "f9a_V")
                {
                    // Verificando el valor de la variable 'direcci'
                    if (direcci == "horizontal_i" || direcci == "vertical_b")
                    {
                        // Creando puntos basados en las coordenadas y el ángulo dado
                        p0 = new Point2d(pts1_v.X + Math.Sin(ANGLE) * e1, pts1_v.Y - Math.Cos(ANGLE) * e1);
                        p1 = new Point2d(pts1_v.X, pts1_v.Y);
                        p2 = new Point2d(pts2_v.X, pts2_v.Y);

                        // Calculando las distancias aproximadas
                        delta0 = (float)Util.Aproximar2Fund(p1.GetDistanceTo(p2), 1);
                        delta1 = (float)Util.Aproximar((float)(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0), 1, diamtro_pl);

                        // Estableciendo puntos para 'acEnt_barra_aux'
                        acEnt_barra_aux.SetPointAt(0, new Point2d(pts1_v.X - Math.Cos(ANGLE) * delta0 + Math.Sin(ANGLE) * (e1 + delta1), pts1_v.Y - Math.Sin(ANGLE) * delta0 - Math.Cos(ANGLE) * (e1 + delta1)));
                        acEnt_barra_aux.SetPointAt(1, new Point2d(pts1_v.X - Math.Cos(ANGLE) * delta0, pts1_v.Y - Math.Sin(ANGLE) * delta0));
                        acEnt_barra_aux.AddVertexAt(2, new Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0);

                        // Estableciendo el valor para la variable 'punto_uso_'
                        Punto_uso_ = acEnt_barra_aux.GetPoint2dAt(2);

                        // Calculando el largo parcial de la barra
                        largo_barra_parcial = "(" + Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) + "+" + Math.Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) + ")";
                        Punto_aux1 = 0;
                        Punto_aux2 = 0;
                    }
                    else if (direcci == "horizontal_d" || direcci == "vertical_a")
                    {
                        // Creando puntos basados en las coordenadas y el ángulo dado
                        p0 = new Point2d(pts1_v.X, pts1_v.Y);
                        p1 = new Point2d(pts2_v.X, pts2_v.Y);
                        p2 = new Point2d(pts2_v.X + Math.Sin(ANGLE) * e1, pts2_v.Y - Math.Cos(ANGLE) * e1);

                        // Calculando las distancias aproximadas
                        delta0 = (float)Util.Aproximar2Fund(p0.GetDistanceTo(p1), 1);
                        delta1 = (float)Util.Aproximar((float)(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0), 1, diamtro_pl);

                        // Estableciendo puntos para 'acEnt_barra_aux'
                        acEnt_barra_aux.SetPointAt(0, new Point2d(pts1_v.X, pts1_v.Y));
                        acEnt_barra_aux.SetPointAt(1, new Point2d(pts2_v.X + Math.Cos(ANGLE) * delta0, pts2_v.Y + Math.Sin(ANGLE) * delta0));
                        acEnt_barra_aux.AddVertexAt(2, new Point2d(pts2_v.X + Math.Cos(ANGLE) * delta0 + Math.Sin(ANGLE) * (e1 + delta1), pts2_v.Y + Math.Sin(ANGLE) * delta0 - Math.Cos(ANGLE) * (e1 + delta1)), 0, 0, 0);

                        // Estableciendo el valor para la variable 'punto_uso_'
                        Punto_uso_ = acEnt_barra_aux.GetPoint2dAt(1);
                        largo_barra_parcial = "(" + Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) + "+" + Math.Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) + ")";
                    }
                    largo_barra_fund = (float)Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0);
                }
                else if (casos == "f10")
                {
                    // Creando puntos basados en las coordenadas y el ángulo dado
                    p0 = new Point2d(pts2_v.X - Math.Sin(ANGLE) * e1, pts2_v.Y + Math.Cos(ANGLE) * e1);
                    p1 = new Point2d(pts2_v.X, pts2_v.Y);
                    p2 = new Point2d(pts1_v.X, pts1_v.Y);
                    p3 = new Point2d(pts1_v.X - Math.Sin(ANGLE) * e1, pts1_v.Y + Math.Cos(ANGLE) * e1);

                    // Inicializando las variables auxiliares
                    delta_aux2 = 0;
                    delta_aux1 = 0;
                    delta0 = (float)Util.Aproximar2Fund(p1.GetDistanceTo(p2), 1);
                    delta1 = (float)Util.Aproximar((float)(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + p2.GetDistanceTo(p3) + delta0), 1, diamtro_pl);

                    // Comprobando si 'delta1' es impar
                    if (delta1 % 2 != 0)
                    {
                        delta_aux1 = delta1 / 2 - 0.5f;
                        delta_aux2 = delta1 / 2 + 0.5f;
                    }
                    else
                    {
                        delta_aux1 = delta1 / 2;
                        delta_aux2 = delta1 / 2;
                    }
                    delta1 = 0;

                    // Estableciendo puntos para 'acEnt_barra_aux'
                    acEnt_barra_aux.SetPointAt(0, new Point2d(pts1_v.X - Math.Cos(ANGLE) * (delta0) - Math.Sin(ANGLE) * (e1 + delta_aux1), pts1_v.Y - Math.Sin(ANGLE) * (delta0) + Math.Cos(ANGLE) * (e1 + delta_aux1)));
                    acEnt_barra_aux.SetPointAt(1, new Point2d(pts1_v.X - Math.Cos(ANGLE) * delta0, pts1_v.Y - Math.Sin(ANGLE) * delta0));
                    acEnt_barra_aux.AddVertexAt(2, new Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0);
                    acEnt_barra_aux.AddVertexAt(3, new Point2d(pts2_v.X - Math.Sin(ANGLE) * (e1 + delta_aux2), pts2_v.Y + Math.Cos(ANGLE) * (e1 + delta_aux2)), 0, 0, 0);

                    // Calculando el largo total y parcial de la barra
                    largo_barra_fund = (float)Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)) + acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0);
                    largo_barra_parcial = "(" + Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) + "+" + Math.Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) + "+" + Math.Round(acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0) + ")";
                    Punto_aux1 = delta_aux1;
                    Punto_aux2 = delta_aux2;

                }
                else if (casos == "f11")
                {
                    // Comprobamos si la dirección es horizontal hacia la izquierda o vertical hacia abajo
                    if (direcci == "horizontal_i" || direcci == "vertical_b")
                    {
                        // Definimos puntos basados en las coordenadas y el ángulo dado
                        p0 = new Point2d(pts1_v.X - Math.Sin(ANGLE) * e1, pts1_v.Y + Math.Cos(ANGLE) * e1);
                        p1 = new Point2d(pts1_v.X, pts1_v.Y);
                        p2 = new Point2d(pts2_v.X, pts2_v.Y);

                        // Calculamos las distancias delta
                        delta0 = (float)Util.Aproximar2Fund(p1.GetDistanceTo(p2), 1);
                        delta1 = (float)Util.Aproximar((float)(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0), 1, diamtro_pl);

                        // Establecemos puntos para 'acEnt_barra_aux'
                        acEnt_barra_aux.SetPointAt(0, new Point2d(pts1_v.X - Math.Cos(ANGLE) * delta0 - Math.Sin(ANGLE) * (e1 + delta1), pts1_v.Y - Math.Sin(ANGLE) * delta0 + Math.Cos(ANGLE) * (e1 + delta1)));
                        acEnt_barra_aux.SetPointAt(1, new Point2d(pts1_v.X - Math.Cos(ANGLE) * delta0, pts1_v.Y - Math.Sin(ANGLE) * delta0));
                        acEnt_barra_aux.AddVertexAt(2, new Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0);

                        // Obtener el punto de uso
                        Punto_uso_ = acEnt_barra_aux.GetPoint2dAt(2);

                        // Calculamos el largo total y parcial de la barra
                        largo_barra_fund = (float)Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0);
                        largo_barra_parcial = $"({Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0)}+{Math.Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0)})";

                        Punto_aux1 = 0;
                        Punto_aux2 = 0;
                    }
                    // Comprobamos si la dirección es horizontal hacia la derecha o vertical hacia arriba
                    else if (direcci == "horizontal_d" || direcci == "vertical_a")
                    {
                        // Definimos puntos basados en las coordenadas y el ángulo dado
                        p0 = new Point2d(pts1_v.X, pts1_v.Y);
                        p1 = new Point2d(pts2_v.X, pts2_v.Y);
                        p2 = new Point2d(pts2_v.X - Math.Sin(ANGLE) * e1, pts2_v.Y + Math.Cos(ANGLE) * e1);

                        // Calculamos las distancias delta
                        delta0 = (float)Util.Aproximar2Fund(p0.GetDistanceTo(p1), 1);
                        delta1 = (float)Util.Aproximar((float)(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0), 1, diamtro_pl);

                        // Establecemos puntos para 'acEnt_barra_aux'
                        acEnt_barra_aux.SetPointAt(0, new Point2d(pts1_v.X, pts1_v.Y));
                        acEnt_barra_aux.SetPointAt(1, new Point2d(pts2_v.X + Math.Cos(ANGLE) * delta0, pts2_v.Y + Math.Sin(ANGLE) * delta0));
                        acEnt_barra_aux.AddVertexAt(2, new Point2d(pts2_v.X + Math.Cos(ANGLE) * delta0 - Math.Sin(ANGLE) * (e1 + delta1), pts2_v.Y + Math.Sin(ANGLE) * delta0 + Math.Cos(ANGLE) * (e1 + delta1)), 0, 0, 0);

                        // Obtener el punto de uso
                        Punto_uso_ = acEnt_barra_aux.GetPoint2dAt(1);

                        // Calculamos el largo total y parcial de la barra
                        largo_barra_fund = (float)Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0);
                        largo_barra_parcial = $"({Math.Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0)}+{Math.Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0)})";
                    }
                }

                Orientacion_ = direcci;
                Tipo = casos;
                Object_poly = acEnt_barra_aux.ObjectId;

                ents.Add(Object_poly);

                Punto_DESFASE0_2 = delta0_2;
                Punto_DESFASE0 = delta0;
                Punto_DESFASE1 = delta1;
                Punto_DESFASE2 = delta2;

                acTrans.Commit();
            }

            // Continue with other 'else if' and 'else' conditions...

        }


        public Polyline dibujar_barra_fund(ref ObjectIdCollection ents, Point2d pt_ini, Point2d pt_fin, ref string st_layer)
        {
            // Obtener el documento, la base de datos y el editor actual
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            Editor Ed = Doc.Editor;

            // Crear una nueva polilínea
            Polyline acPoly = new Polyline();

            using (Transaction tr = DB.TransactionManager.StartTransaction())
            {
                // Acceder a la tabla de bloques
                BlockTable acBlkTbl = (BlockTable)tr.GetObject(DB.BlockTableId, OpenMode.ForRead);

                // Acceder al espacio del modelo para escritura
                BlockTableRecord acBlkTblRec = (BlockTableRecord)tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                // (Este valor parece no utilizarse, considera revisar si es necesario)
                float dista1 = 20;

                // Configurar y dibujar la polilínea
                acPoly.SetDatabaseDefaults();
                acPoly.AddVertexAt(0, pt_ini, 0, 0, 0);
                acPoly.AddVertexAt(1, pt_fin, 0, 0, 0);
                acPoly.Layer = st_layer;

                // Añadir la polilínea al espacio del modelo y a la transacción
                acBlkTblRec.AppendEntity(acPoly);
                tr.AddNewlyCreatedDBObject(acPoly, true);

                // Si 'ents' no es null, añadir el ObjectId de la polilínea
                if (ents != null)
                {
                    ents.Add(acPoly.ObjectId);
                }

                // Confirmar la transacción
                tr.Commit();
            }

            // Devolver la polilínea creada
            return acPoly;
        }
        public Polyline dibujar_barra_fund(ref ObjectIdCollection ents, List<Point2d> ListaPtuntos , ref string st_layer)
        {
            // Obtener el documento, la base de datos y el editor actual
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            Editor Ed = Doc.Editor;

            // Crear una nueva polilínea
            Polyline acPoly = new Polyline();

            using (Transaction tr = DB.TransactionManager.StartTransaction())
            {
                // Acceder a la tabla de bloques
                BlockTable acBlkTbl = (BlockTable)tr.GetObject(DB.BlockTableId, OpenMode.ForRead);

                // Acceder al espacio del modelo para escritura
                BlockTableRecord acBlkTblRec = (BlockTableRecord)tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                

                // Configurar y dibujar la polilínea
                acPoly.SetDatabaseDefaults();
                for (int i = 0; i < ListaPtuntos.Count; i++)
                {
                    acPoly.AddVertexAt(i, ListaPtuntos[i], 0, 0, 0);
                }
                               
                acPoly.Layer = st_layer;
                // Añadir la polilínea al espacio del modelo y a la transacción
                acBlkTblRec.AppendEntity(acPoly);
                tr.AddNewlyCreatedDBObject(acPoly, true);

                // Si 'ents' no es null, añadir el ObjectId de la polilínea
                if (ents != null)
                {
                    ents.Add(acPoly.ObjectId);
                }

                // Confirmar la transacción
                tr.Commit();
            }

            // Devolver la polilínea creada
            return acPoly;
        }
        // Método para dibujar una dimensión
        public void dibujar_dimesion_funda(ref ObjectIdCollection ents, Point3d pt1, Point3d pt2, float ANGLE)
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            Editor Ed = Doc.Editor;
            dimensiones dimension = new dimensiones();

            if (pt1 == new Point3d(0,0,0) || pt2 == new Point3d(0, 0, 0))
            {
                PromptPointOptions pPtOpts = new PromptPointOptions("\n3)Precise el punto Inicial de Rango: ");
                PromptPointResult pPtRes = Doc.Editor.GetPoint(pPtOpts);
                if (pPtRes.Status == PromptStatus.Cancel) return;
                pt1 = pPtRes.Value;

                pPtOpts.Message = "\n4)Precise el punto final de Rango: ";
                pPtOpts.UseBasePoint = true;
                pPtOpts.BasePoint = pt1;
                pPtRes = Doc.Editor.GetPoint(pPtOpts);
                if (pPtRes.Status == PromptStatus.Cancel) return;
                pt2 = pPtRes.Value;
            }

            punto_dim1 = new Point2d(pt1.X, pt1.Y);
            punto_dim2 = new Point2d(pt2.X, pt2.Y);

            using (Transaction tr = DB.TransactionManager.StartTransaction())
            {
                dimension.DrawRotDimension(ref ents, DB, tr, pt1, pt2, 10, "RANGOS", ref ANGLE);
            }
        }

        // Método para dibujar un círculo
        public void dibujar_circulo_fund(ref ObjectIdCollection ents, Point3d pt_ref, ref string tipo, ref float angulo)
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            Editor Ed = Doc.Editor;

            using (Transaction tr = DB.TransactionManager.StartTransaction())
            {
                // Abrir la tabla de bloques en modo lectura
                BlockTable acBlkTbl = (BlockTable)tr.GetObject(DB.BlockTableId, OpenMode.ForRead);

                // Abrir el registro del bloque de Espacio Modelo en modo escritura
                BlockTableRecord acBlkTblRec = (BlockTableRecord)tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                float espacio = 0;

                // Crear un circulo
                Circle acCirc = new Circle();
                acCirc.SetDatabaseDefaults();
                acCirc.Center = new Point3d(pt_ref.X - Math.Sin(angulo) * espacio, pt_ref.Y + Math.Cos(angulo) * espacio, 0);
                acCirc.Radius = 5;
                acCirc.Layer = "0";

                // Añadir el nuevo objeto al registro de la tabla para bloques y a la transacción
                acBlkTblRec.AppendEntity(acCirc);
                tr.AddNewlyCreatedDBObject(acCirc, true);
                ents.Add(acCirc.ObjectId);

                tr.Commit();
            }
        }

        public void dibujar_texto_bloque_fund(float x2_flecha, float y2_flecha, string angle, ObjectIdCollection ents_dt, string AUX_LARGO, string aux_cuatia, string AUX_PARCIAL)
        {
            // Obtener el documento y la base de datos activos
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor acEd = acDoc.Editor;

            BlockReference acBlkRef = null;

            bool aux_texto = false;
            string flecha = "_SCAD_CUANTIA_FUND_NH";

            using (DocumentLock docLock = acDoc.LockDocument())
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Abrir la tabla para bloques en modo lectura
                    BlockTable acBlkTbl = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);

                    if (acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2"))
                    {
                        flecha = "_SCAD_CUANTIA_FUND_NH2";
                    }
                    else
                    {
                        flecha = "_SCAD_CUANTIA_FUND_NH";
                    }

                    // Comprobación si el bloque existe
                    if (acBlkTbl.Has(flecha))
                    {
                        // Abrir el registro del bloque de Espacio Modelo para escritura
                        BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        // Obtener una referencia del bloque
                        BlockTableRecord acBlk = (BlockTableRecord)acTrans.GetObject(acBlkTbl[flecha], OpenMode.ForRead, false);

                        // Crear una referencia a bloque
                        acBlkRef = new BlockReference(new Point3d(x2_flecha, y2_flecha, 0), acBlk.ObjectId);

                        // Establecer rotación
                        acBlkRef.Rotation = double.Parse(angle);

                        // Añadir el nuevo objeto al registro de la tabla para bloques y a la transacción
                        ObjectId objId = acBlkTblRec.AppendEntity(acBlkRef);
                        ents_dt.Add(objId);

                        acTrans.AddNewlyCreatedDBObject(acBlkRef, true);

                        // Si el bloque contiene definiciones de atributo
                        if (acBlk.HasAttributeDefinitions)
                        {
                            foreach (ObjectId acObjId in acBlk)
                            {
                                DBObject acObj = acTrans.GetObject(acObjId, OpenMode.ForRead, false);
                                AttributeDefinition acAttDef = acObj as AttributeDefinition;

                                if (acAttDef != null && !acAttDef.Constant)
                                {
                                    // Crear una referencia de atributo
                                    AttributeReference acAttRef = new AttributeReference();
                                    acAttRef.SetAttributeFromBlock(acAttDef, acBlkRef.BlockTransform);

                                    // Actualizar el texto de atributo multilínea (solo para AutoCAD 2008 o superior)
                                    if (acAttDef.IsMTextAttributeDefinition)
                                    {
                                        acAttRef.UpdateMTextAttribute();
                                    }

                                    int suma_plus = 0;
                                    int suma_largo = 0;

                                    if (acAttRef.Tag == "CUANTIA")
                                    {
                                        acAttRef.TextString = aux_cuatia;

                                        Point2d POTI3 = new Point2d(acAttRef.AlignmentPoint.X, acAttRef.AlignmentPoint.Y);
                                        if (aux_cuatia.ToLower().Contains("f")) suma_plus = 5;
                                        if (aux_cuatia.Contains("+")) suma_plus = 8;
                                        if (AUX_LARGO.Length > 5) suma_largo = 13;

                                        acAttRef.AlignmentPoint = new Point3d(acAttRef.AlignmentPoint.X + (-suma_plus - suma_largo) * Math.Cos(double.Parse(angle)), acAttRef.AlignmentPoint.Y + (-suma_plus - suma_largo) * Math.Sin(double.Parse(angle)), 0);
                                    }
                                    else if (acAttRef.Tag == "LARGO")
                                    {
                                        acAttRef.TextString = AUX_LARGO;
                                    }
                                    else if (acAttRef.Tag == "PARCIAL")
                                    {
                                        acAttRef.TextString = AUX_PARCIAL;
                                    }

                                    // Agregar la referencia de atributo a la referencia de bloque y a la transacción
                                    acBlkRef.AttributeCollection.AppendAttribute(acAttRef);
                                    acTrans.AddNewlyCreatedDBObject(acAttRef, true);
                                }
                            }
                        }

                        // Confirmar los cambios
                        acTrans.Commit();
                    }
                    else
                    {
                        // System.Windows.Forms.MessageBox.Show("NO se encontró el bloque de texto", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        aux_texto = true;
                        return;
                    }
                }
            }
        }


        public (Polyline, ObjectIdCollection) DibujarBarraFund(ObjectIdCollection ents, Point2d pt_ini, Point2d pt_fin, string st_layer)
        {
            // Obtener el documento, la base de datos y el editor actual
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            Editor Ed = Doc.Editor;

            // Crear una nueva polilínea
            Polyline acPoly = new Polyline();

            using (Transaction tr = DB.TransactionManager.StartTransaction())
            {
                // Acceder a la tabla de bloques
                BlockTable acBlkTbl = (BlockTable)tr.GetObject(DB.BlockTableId, OpenMode.ForRead);

                // Acceder al espacio del modelo para escritura
                BlockTableRecord acBlkTblRec = (BlockTableRecord)tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                // (Este valor parece no utilizarse, considera revisar si es necesario)
                float dista1 = 20;

                // Configurar y dibujar la polilínea
                acPoly.SetDatabaseDefaults();
                acPoly.AddVertexAt(0, pt_ini, 0, 0, 0);
                acPoly.AddVertexAt(1, pt_fin, 0, 0, 0);
                acPoly.Layer = st_layer;

                // Añadir la polilínea al espacio del modelo y a la transacción
                acBlkTblRec.AppendEntity(acPoly);
                tr.AddNewlyCreatedDBObject(acPoly, true);

                // Si 'ents' no es null, añadir el ObjectId de la polilínea
                if (ents != null)
                {
                    ents.Add(acPoly.ObjectId);
                }

                // Confirmar la transacción
                tr.Commit();
            }

            // Devolver la polilínea creada
            return (acPoly, ents);
        }

        public Point3d intesrcion_barra_dimension(Point2d pto1_barra, Point2d pto2_barra, Point2d pto1_dim, Point2d pto2_dim)
        {
            Point3d pto_inter = new Point3d();

            Point3dCollection pt1 = new Point3dCollection();

            // Dibuja la barra
            (Polyline acPoly1, ObjectIdCollection aux) = DibujarBarraFund(null, pto1_barra, pto2_barra, "BARRAS");
            (Polyline acPoly2, ObjectIdCollection aux2) = DibujarBarraFund(null, pto1_dim, pto2_dim, "BARRAS");

            // Busca la intersección de las barras
            acPoly1.IntersectWith(acPoly2, Intersect.OnBothOperands, pt1, IntPtr.Zero, IntPtr.Zero);

            acPoly1.Erase();
            acPoly2.Erase();

            // Si hay una intersección, se actualiza el punto de intersección
            if (pt1.Count > 0)
            {
                pto_inter = pt1[0];
            }
            else
            {
                //  MessageBox.Show("No existe intersección entre barra y dimensión", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return pto_inter;
        }


        public void dibujar_texto_pl_reactor(ObjectIdCollection ents, ObjectId obj_, ref string aux_cuatia, string AUX_LARGO, ref string AUX_PARCIAL)
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            Editor Ed = Doc.Editor;

            using (Transaction tr = DB.TransactionManager.StartTransaction())
            {
                try
                {
                    // Buscar grupo y nombre del grupo
                    CODIGOS_GRUPOS GRUPO_ = new CODIGOS_GRUPOS();
                    ObjectId[] acObjId_grup = GRUPO_.buscar_grupo(obj_);
                    string nombre_grupo_ = GRUPO_.buscar_nombre_grupo(ref obj_);

                    foreach (ObjectId idObj in acObjId_grup)
                    {
                        // Si es una inserción, modifica el bloque de texto
                        if (idObj.ObjectClass.DxfName == "INSERT")
                        {
                            BlockReference acEnt = (BlockReference)tr.GetObject(idObj, OpenMode.ForWrite);
                            string[] datos_estribo_valores = new string[5];
                            ModificarBloqueTextoFund(aux_cuatia, AUX_LARGO, AUX_PARCIAL, idObj, ref ents);
                        }
                    }

                    tr.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                {
                    tr.Dispose();
                    //MessageBox.Show("Error: 'reac_modif_barra_final'\n" + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public void ModificarBloqueTextoFund(string aux_cuatia, string AUX_LARGO, string AUX_PARCIAL, object obj_, ref ObjectIdCollection ents_dt)
        {
            string flecha = "";

            // Obtain the active document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the block table for reading
                BlockTable acBlkTbl = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);

                // Determine which flecha value to use
                flecha = acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2") ? "_SCAD_CUANTIA_FUND_NH2" : "_SCAD_CUANTIA_FUND_NH";

                // Check if the block exists
                if (acBlkTbl.Has(flecha))
                {
                    BlockReference acBlkRef = (BlockReference)acTrans.GetObject((ObjectId)obj_, OpenMode.ForRead, false, true);
                    AttributeCollection acAttCol = acBlkRef.AttributeCollection;

                    foreach (ObjectId acObjId in acAttCol)
                    {
                        AttributeReference acAttRef = (AttributeReference)acTrans.GetObject(acObjId, OpenMode.ForWrite);

                        int suma_plus = 0;
                        int suma_largo = 8;
                        if (acAttRef.Tag == "CUANTIA")
                        {
                            acAttRef.TextString = aux_cuatia;
                        }
                        else if (acAttRef.Tag == "LARGO")
                        {
                            acAttRef.TextString = "L=" + AUX_LARGO;
                        }
                        else if (acAttRef.Tag == "PARCIAL")
                        {
                            acAttRef.TextString = AUX_PARCIAL;
                        }
                    }

                    acTrans.Commit();
                }
            }
        }

    }

}
