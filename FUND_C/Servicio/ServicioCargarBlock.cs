using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VARIOS;


namespace FUND_C.Servicio
{
    public class ServicioCargarBlock
    {
        public static bool Bloque_InsertarLeaderFundacion()
        {
            try
            {
                // Obtener el documento activo y la base de datos
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;

                // Bloquear el documento durante la operación
                using (doc.LockDocument())
                {
                    // Nombre del bloque que deseamos verificar/insertar
                    string flecha = "_SCAD_CUANTIA_FUND_NH2";

                    // Iniciar una transacción para trabajar con la base de datos de AutoCAD
                    using (Transaction acTrans = db.TransactionManager.StartTransaction())
                    {
                        // Abrir la tabla de bloques en modo lectura
                        BlockTable acBlkTbl = (BlockTable)acTrans.GetObject(db.BlockTableId, OpenMode.ForRead);

                        // Comprobar si el bloque existe en la tabla
                        if (!acBlkTbl.Has(flecha))
                        {
                            // Supongo que 'atributos' es una clase definida previamente en tu proyecto
                            atributos VARIOS_ = new atributos();

                            // Verificar si el archivo del bloque existe en el sistema
                            if (File.Exists(@"C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg"))
                            {
                                // Insertar el bloque en el dibujo
                                VARIOS_.InsertBlock(@"C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg", flecha);
                            }

                            // Comprobar nuevamente si el bloque se insertó correctamente
                            if (!acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2"))
                            {
                                MessageBox.Show("Insertar bloque de texto '_SCAD_CUANTIA_FUND_NH2'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                // La línea siguiente es equivalente al 'GoTo final2' en VB.NET.
                                // Si tienes alguna lógica específica para el label 'final2', debes reestructurar tu código en C#.
                                // Aquí simplemente saldrá del método.
                                return false;
                            }
                        }

                        // Confirmar los cambios
                        acTrans.Commit();
                    }
                }//fin 'doc.LockDocument()'
            }//fin  'try'
            catch (Exception)
            {
                MessageBox.Show("Error al Insertar bloque de texto '_SCAD_CUANTIA_FUND_NH2'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // La línea siguiente es equivalente al 'GoTo final2' en VB.NET.
                // Si tienes alguna lógica específica para el label 'final2', debes reestructurar tu código en C#.
                // Aquí simplemente saldrá del método.
                return false;
            }


            return true;
        }

    }
}
