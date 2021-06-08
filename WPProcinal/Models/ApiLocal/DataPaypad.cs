using Newtonsoft.Json;

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
        public string MensajeDatafono { get; set; }
        public bool ModalPlate { get; set; }
        public int CodCinema { get; set; }

        [JsonProperty("4Code")]
        public int Code4 { get; set; }
        public string NitPromotora { get; set; }
        public string Nit { get; set; }
        public string PublicityRoute { get; set; }
        public string MensajeSinDineroInitial { get; set; }
        public bool ModalBioseguridad { get; set; }
        public string MensajeSinDinero { get; set; }
        public string UrlImages { get; set; }
        public string RecorderRoute { get; set; }
        public bool Speack { get; set; }
        public string ProductsURL { get; set; }
        public string MensajeError { get; set; }
        public string MensajeCensura { get; set; }
        public string MensajeCinefans { get; set; }
        public string MensajeURL { get; set; }
        public string MensajeUbicaciones { get; set; }
        public string TiposAutos { get; set; }
        public string ScoreKey { get; set; }
        public DataDatafono DataDatafono { get; set; }
        public string tercero { get; set; }
        //public DataDatafono Data_Datafono { get; set; }
        public DESARROLLO desarrollo { get; set; }
        public PRODUCCION produccion { get; set; }
 
        public AMBIENTE AMBIENTE { get; set; }
        public int MaxSillas { get; set; }

        public void DefinirAmbiente(bool iS_PRODUCTION)
        {
            if (iS_PRODUCTION)
            {
                AMBIENTE = produccion;
            }
            else
            {
                AMBIENTE = desarrollo;
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
        public string Delimitador { get; set; }
        public string IdentificadorInicio { get; set; }
        public string TipoOperacion { get; set; }
        public string CodigoUnico { get; set; }
    }
}

