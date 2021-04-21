using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Models.ApiLocal
{
    public class DataPaypad
    {
        public bool State { get; set; }
        public bool StateAceptance { get; set; }
        public bool StateDispenser { get; set; }
        public string Message { get; set; }
        public object ListImages { get; set; }
        public object Invoicedata { get; set; }
        public bool StateUpdate { get; set; }
        public PaypadConfiguration PaypadConfiguration { get; set; }
    }
    public class PaypadConfiguration
    {
        public int paypaD_ID { get; set; }
        public string bilL_PORT { get; set; }
        public string coiN_PORT { get; set; }
        public string unifieD_PORT { get; set; }
        public string printeR_PORT { get; set; }
        public string dispenseR_CONFIGURATION { get; set; }
        public string localdB_PATH { get; set; }
        public int publicitY_TIMER { get; set; }
        public string generiC_TIMER { get; set; }
        public string modaL_TIMER { get; set; }
        public string inactivitY_TIMER { get; set; }
        public string extrA_DATA { get; set; }
        public ExtraData ExtrA_DATA { get; set; }
        public bool enablE_CARD { get; set; }
        public bool enablE_VALIDATE_PERIPHERALS { get; set; }
        public bool iS_UNIFIED { get; set; }
        public bool iS_PRODUCTION { get; set; }

        [JsonProperty("keyS_PATH")]
        public string updater_PATH { get; set; }
        public string imageS_PATH { get; set; }
        public string scanneR_PORT { get; set; }

        public void DeserializarExtraData()
        {
            ExtrA_DATA = JsonConvert.DeserializeObject<ExtraData>(extrA_DATA);
        }
    }
    public class ExtraData
    {
        public string mensajeDatafono { get; set; }
        public bool modalPlate { get; set; }
        public int codCinema { get; set; }

        [JsonProperty("4Code")]
        public int Code4 { get; set; }
        public string nitPromotora { get; set; }
        public string nit { get; set; }
        public string publicityRoute { get; set; }
        public string mensajeSinDineroInitial { get; set; }
        public bool modalBioseguridad { get; set; }
        public string mensajeSinDinero { get; set; }
        public string urlImages { get; set; }
        public string recorderRoute { get; set; }
        public bool speack { get; set; }
        public string productsURL { get; set; }
        public string mensajeError { get; set; }
        public string mensajeCensura { get; set; }
        public string mensajeCinefans { get; set; }
        public string mensajeURL { get; set; }
        public string mensajeUbicaciones { get; set; }
        public string tiposAutos { get; set; }
        public string scoreKey { get; set; }
        public string dataDatafono { get; set; }
        public string tercero { get; set; }
        public DataDatafono DataDatafono { get; set; }

        public string desarrollo { get; set; }
        public string produccion { get; set; }
        public DESARROLLO DESARROLLO { get; set; }
        public PRODUCCION PRODUCCION { get; set; }

        public AMBIENTE AMBIENTE { get; set; }
        public void DeserializeDataDatafono()
        {
            DataDatafono = JsonConvert.DeserializeObject<DataDatafono>(dataDatafono);
        }

        public void DeserializeAmbiente()
        {
            DESARROLLO = JsonConvert.DeserializeObject<DESARROLLO>(desarrollo);
            PRODUCCION = JsonConvert.DeserializeObject<PRODUCCION>(produccion);
        }


        public void DefinirAmbiente(bool iS_PRODUCTION)
        {
            if (iS_PRODUCTION)
            {
                AMBIENTE = PRODUCCION;
            }
            else
            {
                AMBIENTE = DESARROLLO;
            }
        }
    }
    public class DESARROLLO : AMBIENTE
    {

    }

    public class PRODUCCION : AMBIENTE
    {

    }
    public class AMBIENTE
    {
        public int puntoVenta { get; set; }
        public string webServiceUrl { get; set; }
        public string scoreService { get; set; }
    }
    public class DataDatafono
    {
        public string delimitador { get; set; }
        public string identificadorInicio { get; set; }
        public string tipoOperacion { get; set; }
        public string codigoUnico { get; set; }
    }
}

