using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace FUND_C.DTO
{
    public class BarraFundacionesDTO
    {
        public int NumberOfVertices { get; internal set; }
        public List<SegmentoDTO> ListaSegmento { get;  set; }

        public BarraFundacionesDTO()
        {
            ListaSegmento = new List<SegmentoDTO>();
        }
    }
}
