using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Charact
{
    public partial class Form1 : Form
    {
        struct Tck
        {
            public Tck(double x, double t, double ut, double ux, double u) { this.x = x; this.t = t; this.ut = ut; this.ux = ux; this.u = u; }

            public double x;
            public double t;
            public double u;
            public double ut;
            public double ux;
        };

        double a = 100;
        double xmax = 9.423;
        //double xmax = 10;
        double hx = 0.1;
        Dictionary<int, Tck> chet;
        Dictionary<int, Tck> nechet;
        int max_index;
        bool is_chet = false;

        private double mu1(double t) {
            return 0;
        }

        private double mu2(double t)
        {
            //return Math.Sin(10);
            return 0;
        }

        private double fi(double x)
        {
            return Math.Sin(x);
        }

        private double psi(double x)
        {
            return 0;
        }

        public Form1()
        {
            InitializeComponent();
            chet = new Dictionary<int, Tck>();
            nechet = new Dictionary<int, Tck>();
            chart1.ChartAreas[0].AxisX.Maximum = xmax;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Minimum = -1;
            chart1.ChartAreas[0].AxisY.Maximum = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            init_solve();

            timer1.Start(); 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (is_chet)
                chet_zap();
            else
                nechet_zap();
            is_chet = !is_chet;
        }

        private void chet_zap()
        {
            chet.Clear();
            chart1.Series[0].Points.Clear();
            int index = 1;
            while (index <= max_index - 1)
            {
                Tck t = triangle(nechet[index], nechet[index+1]);
                chet.Add(index, t);
                //chart1.Series[0].Points.AddXY(t.x, t.u);

                index++;
            }

            //Tck E = new Tck((-a * (chet[1].t - nechet[1].t) + nechet[1].x), );
            double ute = (mu1(chet[1].t + 0.001) - mu1(chet[1].t)) / 0.001;
            Tck E = new Tck(0, chet[1].t, ute, ((ute - nechet[1].ut) / (-a) + nechet[1].ux), mu1(chet[1].t));
            chet.Add(0, E);
            chart1.Series[0].Points.AddXY(0, E.u);

            index = 1;
            while (index <= max_index - 1)
            {
                
                chart1.Series[0].Points.AddXY(chet[index].x, chet[index].u);

                index++;
            }

            double utf = (mu2(chet[1].t + 0.001) - mu2(chet[1].t)) / 0.001;
            Tck F = new Tck(xmax, chet[max_index - 1].t, utf, ((utf - nechet[max_index].ut) / (a) + nechet[max_index].ux), mu2(chet[max_index - 1].t));
            chet.Add(max_index, F);
            chart1.Series[0].Points.AddXY(xmax, F.u);
        }

        private void nechet_zap()
        {
            nechet.Clear();
            chart1.Series[0].Points.Clear();
            int index = 1;
            while (index <= max_index)
            {
                Tck t = triangle(chet[index - 1], chet[index]);
                chart1.Series[0].Points.AddXY(t.x, t.u);
                nechet.Add(index, t);

                index++;
            }
        }

        private void init_solve()
        {
            double x = 0;
            double t = 0;
            int index = 0;
            while (x <= xmax) {
                Tck tck = new Tck(x, t, psi(x), (fi(x + 0.001) - fi(x)) / 0.001, fi(x));
                chet.Add(index, tck);

                index++;
                x += hx;
            }
            max_index = index - 1;

            /*while (x < xmax)
            {
                Tck tck = new Tck(x, t, psi(x), (fi(x + 0.001) - fi(x)) / 0.001, fi(x));
                chet.Add(index, tck);

                index++;
                x += hx;
            }
            Tck tck1 = new Tck(x, t, psi(x), (fi(x + 0.001) - fi(x)) / 0.001, mu2(t));
            chet.Add(index, tck1);

            max_index = index;*/

            chart1.Series[0].Points.Clear();
            /*for (double x = 0; x <= xmax; x += hx)
            {
                chart1.Series[0].Points.AddXY(x, fi(x));
            }*/

            for (index = 0; chet.ContainsKey(index); index++)
            {
                chart1.Series[0].Points.AddXY(chet[index].x, chet[index].u);
            }
        }

        private Tck triangle(Tck A, Tck B) 
        {
            double x = (a * (B.t - A.t) + A.x + B.x) / 2;
            double t = (x - A.x + a * A.t) / a;
            double ut = (a * (B.ux - A.ux) + A.ut + B.ut) / 2;
            double ux = (ut - A.ut + a * A.ux) / a;
            double u = A.u + (ut * (t - A.t) + ux * (x - A.x));
            return new Tck(x, t, ut, ux, u);
        }
    }
}
