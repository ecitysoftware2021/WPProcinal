using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WPProcinal.Classes
{
    class TimerTiempo
    {
        public Action<string> CallBackTimer;
        public Action<int> CallBackClose;
        public Action<int> CallBackStop;

        public int Minutos;
        public int Segundos;

        Timer timer;
        public TimerTiempo(string tiempo)
        {
            timer = new Timer();
            Minutos = int.Parse(tiempo.Split(':')[0]);
            Segundos = int.Parse(tiempo.Split(':')[1]);
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerTick);
            timer.Start();

        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            CallBackStop = response =>
            {
                timer.Stop();
                GC.Collect();
               
            };

            if (Minutos >= 1)
            {
                Segundos--;
                if (Segundos == 0)
                {
                    Minutos--;
                    Segundos = 59;
                }
            }
            else
            {
                Segundos--;
                if (Segundos == 0)
                {
                    timer.Stop();
                    CallBackClose?.Invoke(1);
                    GC.Collect();
                }
            }
            CallBackTimer?.Invoke(string.Concat(Minutos.ToString().PadLeft(2, '0'), ":", Segundos.ToString().PadLeft(2, '0')));
        }
    }
}
