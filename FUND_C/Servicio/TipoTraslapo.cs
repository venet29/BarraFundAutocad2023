using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUND_C.Servicio
{
    public class TipoTraslapo
    {
        public string Tipo_losa { get; private set; }
        public string Tipo_direccion { get; private set; }
        public TipoTraslapo()
        {
            
        }
        public bool ObtenerTiposIzquierdo(string tipo_losa_Inicial, string tipo_direccion_Inicial)
        {
            try
            {
                Tipo_losa = "";
                Tipo_direccion = "";

                // Evaluar el tipo de losa basándose en el primer elemento de 'datos_losa2'
                switch (tipo_losa_Inicial)
                {
                    case "f10":
                        Tipo_losa = "f11";

                        // Evaluar la dirección de la losa basándose en el tercer elemento de 'datos_losa2'
                        switch (tipo_direccion_Inicial)
                        {
                            case "horizontal_i":
                            case "horizontal_d":
                                Tipo_direccion = "horizontal_i";
                                break;
                            case "vertical_b":
                            case "vertical_a":
                                Tipo_direccion = "vertical_b";
                                break;
                        }
                        break;

                    case "f11":
                        switch (tipo_direccion_Inicial)
                        {
                            case "horizontal_i":
                                Tipo_losa = "f11";
                                Tipo_direccion = "horizontal_i";
                                break;
                            case "horizontal_d":
                                Tipo_losa = "f3";
                                Tipo_direccion = "horizontal_i";
                                break;
                            case "vertical_a":
                            case "vertical_b":
                                Tipo_losa = "f11";
                                Tipo_direccion = "vertical_b";
                                break;
                        }
                        break;

                    case "f3":
                        Tipo_losa = "f3";
                        switch (tipo_direccion_Inicial)
                        {
                            case "horizontal_i":
                            case "horizontal_d":
                                Tipo_direccion = "horizontal_i";
                                break;
                            case "vertical_b":
                            case "vertical_a":
                                Tipo_direccion = "vertical_b";
                                break;
                        }
                        break;

                    case "f9a":
                        Tipo_losa = "f9a_V";
                        switch (tipo_direccion_Inicial)
                        {
                            case "horizontal_i":
                            case "horizontal_d":
                                Tipo_direccion = "horizontal_i";
                                break;
                            case "vertical_b":
                            case "vertical_a":
                                Tipo_direccion = "vertical_b";
                                break;
                        }
                        break;

                    case "f9a_V":
                        switch (tipo_direccion_Inicial)
                        {
                            case "horizontal_i":
                                Tipo_losa = "f9a_V";
                                Tipo_direccion = "horizontal_i";
                                break;
                            case "horizontal_d":
                                Tipo_losa = "f3";
                                Tipo_direccion = "horizontal_i";
                                break;
                            case "vertical_b":
                                Tipo_losa = "f9a_V";
                                Tipo_direccion = "vertical_b";
                                break;
                            case "vertical_a":
                                Tipo_losa = "f3";
                                Tipo_direccion = "vertical_b";
                                break;
                        }
                        break;

                    default:
                        Util.ErrorMsg("Tipo de barra distinta a la base de datos");
                        return false;
                }


            }
            catch (Exception ex)
            {
                Util.ErrorMsg($"Error en 'ObtenerTipos'. ex:{ex.Message}");
                return false;
            }
            return true;
        }


    }
}
