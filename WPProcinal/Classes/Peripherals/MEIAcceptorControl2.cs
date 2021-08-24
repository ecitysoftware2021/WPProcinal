using MPOST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPProcinal.Classes.Peripherals
{
    public class MEIAcceptorControl2
    {
        private Acceptor meiAcceptor;
        public  Action<EMEIMEssages> callbackMeiMessages;
        public  Action<double> callbackBillAccepted;

        public Dictionary<string, string> MeiMessagesHomologate = new Dictionary<string, string>
        {
            {"MeiConnected","Aceptador Conectado" },
            {"MeiBillEscrow","Billete Ingresado" },
            {"MeiBillStacked","Billete Contabilizado" },
            {"MeiBillRejected","No acepto Billetes falso" },
            {"MeiCashBoxRemoved","Baúl Retirado" },
            {"MeiCashBoxAttached","Baúl Reincorporado" },
            {"MeiJamDetected","Aceptador Atascado" },
            {"MeiJamResolved","Aceptador Desatascado" },
            {"MeiCheated","Engaño Detectado" },
            {"MeiDisconnected","Aceptador Desconectado" },
            {"MeiPauseDetected","Aceptador Pausado" },
            {"MeiPauseResolved","Aceptador Despausado" },
            {"MeiPowerUp","Aceptador Iniciando" },
            {"MeiPowerUpComplete","Aceptador Iniciado" },
            {"MeiBillInScrowOnPowerUp","Billete en el aceptador" },
            {"MeiStackerFull","Baul Lleno" },
            {"MeiStackerFullResolved","Espacio liberado en el baul" },
            {"MeiStallDetected","Aceptación Detenida" },
            {"MeiStallResolved","Aceptación Reactivada" }
        };

        public MEIAcceptorControl2()
        {
            try
            {
                if (meiAcceptor == null)
                {
                    meiAcceptor = new Acceptor();
                }

                meiAcceptor.OnConnected += new ConnectedEventHandler(MeiConnected);
                meiAcceptor.OnEscrow += new EscrowEventHandler(MeiBillEscrow);
                meiAcceptor.OnStackedWithDocInfo += new StackedWithDocInfoEventHandler(MeiBillStacked);
                meiAcceptor.OnRejected += new RejectedEventHandler(MeiBillRejected);
                meiAcceptor.OnCashBoxRemoved += new CashBoxRemovedEventHandler(MeiCashBoxRemoved);
                meiAcceptor.OnCashBoxAttached += new CashBoxAttachedEventHandler(MeiCashBoxAttached);
                meiAcceptor.OnJamDetected += new JamDetectedEventHandler(MeiJamDetected);
                meiAcceptor.OnJamCleared += new JamClearedEventHandler(MeiJamResolved);
                meiAcceptor.OnCheated += new CheatedEventHandler(MeiCheated);
                meiAcceptor.OnDisconnected += new DisconnectedEventHandler(MeiDisconnected);
                //meiAcceptor.OnNoteRetrieved += new NoteRetrievedEventHandler(MeiNoteRetrieve);
                meiAcceptor.OnPauseDetected += new PauseDetectedEventHandler(MeiPauseDetected);
                meiAcceptor.OnPauseCleared += new PauseClearedEventHandler(MeiPauseResolved);
                meiAcceptor.OnPowerUp += new PowerUpEventHandler(MeiPowerUp);
                meiAcceptor.OnPowerUpComplete += new PowerUpCompleteEventHandler(MeiPowerUpComplete);
                meiAcceptor.OnPUPEscrow += new PUPEscrowEventHandler(MeiBillInScrowOnPowerUp);
                meiAcceptor.OnStackerFull += new StackerFullEventHandler(MeiStackerFull);
                meiAcceptor.OnStackerFullCleared += new StackerFullClearedEventHandler(MeiStackerFullResolved);
                meiAcceptor.OnStallDetected += new StallDetectedEventHandler(MeiStallDetected);
                meiAcceptor.OnStallCleared += new StallClearedEventHandler(MeiStallResolved);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Instanciando la clase MEIAcceptorControl", EError.Device, ELevelError.Medium);
            }
        }

        /// <summary>
        /// La parada detectada se resolvió
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiStallResolved(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiStallResolved);
        }

        /// <summary>
        /// Se detectó una parada en el billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiStallDetected(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiStallDetected);
        }

        /// <summary>
        /// Se retiraron billetes del baul luego de estar lleno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiStackerFullResolved(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiStackerFullResolved);
        }

        /// <summary>
        /// Se llenó el baul del billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiStackerFull(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiStackerFull);
        }

        /// <summary>
        /// Billete en el scrow mientras se inicia el billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiBillInScrowOnPowerUp(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiBillInScrowOnPowerUp);
        }

        /// <summary>
        /// Billetero inicia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiPowerUpComplete(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiPowerUpComplete);
        }

        /// <summary>
        /// Iniciando el billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiPowerUp(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiPowerUp);
        }

        /// <summary>
        /// Billetero sale de modo pausa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiPauseResolved(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiPauseResolved);
        }

        /// <summary>
        /// Billetero entra en modo pausa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiPauseDetected(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiPauseDetected);
        }

        /// <summary>
        /// Billetero desconectado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiDisconnected(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiDisconnected);
        }

        /// <summary>
        /// Engaño detectado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiCheated(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiCheated);
        }

        /// <summary>
        /// Atasco resuelto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiJamResolved(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiJamResolved);
        }

        /// <summary>
        /// Atasco Detectado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiJamDetected(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiJamDetected);
        }

        /// <summary>
        /// Baul ingresado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiCashBoxAttached(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiCashBoxAttached);
        }

        /// <summary>
        /// Baul retirado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiCashBoxRemoved(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiCashBoxRemoved);
        }

        /// <summary>
        /// Billete Rechazado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiBillRejected(object sender, EventArgs e)
        {
            callbackMeiMessages?.Invoke(EMEIMEssages.MeiBillRejected);
        }

        /// <summary>
        /// Billete Guardado en el baul
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiBillStacked(object sender, StackedEventArgs e)
        {
            try
            {
                var acep = sender as Acceptor;

                if (acep.Bill != null)
                {
                    callbackBillAccepted?.Invoke(acep.Bill.Value);
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "La clase MeiBillStacked", EError.Device, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Billete Ingresado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiBillEscrow(object sender, EventArgs e)
        {
            try
            {
                Bill aceptado = (sender as Acceptor).Bill;


            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "La clase MeiBillEscrow", EError.Device, ELevelError.Medium);
            }
        }

        /// <summary>
        /// Puerto abierto y listo para aceptar billetes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeiConnected(object sender, EventArgs e)
        {
            try
            {
                meiAcceptor.AutoStack = true;
                callbackMeiMessages?.Invoke(EMEIMEssages.MeiConnected);
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "La clase MeiConnected", EError.Device, ELevelError.Medium);
            }
        }

        #region "Métodos"
        public void OpenAcceptor(string acceptorPort)
        {
            try
            {
                if (!meiAcceptor.Connected)
                {
                    meiAcceptor.Open(acceptorPort);
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "La clase OpenAcceptor", EError.Device, ELevelError.Medium);
            }
        }
        public void InitAcceptance()
        {
            try
            {
                if (meiAcceptor.Connected)
                {
                    meiAcceptor.EnableAcceptance = true;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "La clase InitAcceptance", EError.Device, ELevelError.Medium);
            }
        }

        public void StopAcceptance()
        {
            try
            {
                if (meiAcceptor.Connected)
                {
                    meiAcceptor.EnableAcceptance = false;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "La clase StopAcceptance", EError.Device, ELevelError.Medium);
            }
        }

        public void CloseAcceptor()
        {
            try
            {
                if (meiAcceptor.Connected)
                {
                    meiAcceptor.Close();
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "La clase CloseAcceptor", EError.Device, ELevelError.Medium);
            }
        }
        #endregion
    }
}
