using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

//using NUnit.Framework;
//using NUnit.Framework.Constraints;

namespace QATools
{
    [TestFixture]
    public class MathFunctionTest
    {
        [Test]
        public void CalculateSplineCoefficients()
        {
            //double[] x = new[] {0,219.0, 583, 947, 1314, 1680};
            double[] x = new double[] {0, 1, 2, 3, 4, 5};
            double[] y = new double[] {140, 145, 146, 152, 153, 157};
            CubicSpline spline = new CubicSpline();
            spline.BuildSpline(x, y, 4);

            Console.WriteLine($"Polynomials ({spline.splines.Length} #):");
            foreach (var item in spline.splines)
            {

                //Console.WriteLine("a= " + item.a + "; b=" + item.b + "; c= " + item.c + "; d= " + item.d);
                Console.WriteLine(item.a + ";" + item.b + ";" + item.c + ";" + item.d);
            }

            double d = -01;
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"{d}: {spline.Interpolate(d)}");

                i += 1;
                d += 0.1;
            }
        }

        [Test]
        public void Test2()
        {
            double[] timePoints = new double[] { 0, 6097,  14833, 23569, 32377, 41161};
            double[] prices = new double[] { 140, 145, 146, 152, 153, 157 };
            double[] scaledPrices = new double[prices.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                scaledPrices[i] = prices[i] / 24.0;
            }

            CubicSpline spline = new CubicSpline();
            spline.BuildSpline(timePoints, scaledPrices, prices.Length);

            int limit = (int)(timePoints[timePoints.Length-1]*1.5)/10;
            double[] times = new double[limit];
            double[] values = new double[limit];
            
            for (int i = 0; i < limit; i++)
            {
                times[i] = (i-50)*10;
                double value = spline.Interpolate(times[i]) * 24;
                values[i] = value;
                //Console.WriteLine($"{i*10}: {value}");
            }
        }

        // Cubic spline interpolation

        public class CubicSpline
        {
            public SplineTuple[] splines; // spline

            public struct SplineTuple
            {
                public double a, b, c, d, x;
            }

           //Build spline
           //x - points, orders by increation 
           //y - function value in each point
           //n - number of points

            public void BuildSpline(double[] x, double[] y, int n)
            {
                //spline initialization 
                splines = new SplineTuple[n];
                for (int i = 0; i < n; ++i)
                {
                    splines[i].x = x[i];
                    splines[i].a = y[i];
                }
                splines[0].c = splines[n - 1].c = 0.0;
                
                //calculate splines coefficients c[i] using method for tridiagonal matrices
                //calculate coefficients -  tridiagonal matrix algorithm (Вычисление прогоночных коэффициентов - прямой ход метода прогонки)
                double[] alpha = new double[n - 1];
                double[] beta = new double[n - 1];
                alpha[0] = beta[0] = 0.0;
                for (int i = 1; i < n - 1; ++i)
                {
                    double hi = x[i] - x[i - 1];
                    double hi1 = x[i + 1] - x[i];
                    double A = hi;
                    double C = 2.0*(hi + hi1);
                    double B = hi1;
                    double F = 6.0*((y[i + 1] - y[i])/hi1 - (y[i] - y[i - 1])/hi);
                    double z = (A*alpha[i - 1] + C);
                    alpha[i] = -B/z;
                    beta[i] = (F - A*beta[i - 1])/z;
                }

                //determination
                for (int i = n - 2; i > 0; --i)
                {
                    splines[i].c = alpha[i]*splines[i + 1].c + beta[i];
                }

                // we have c[i], will calculate b[i] and d[i]
                for (int i = n - 1; i > 0; --i)
                {
                    double hi = x[i] - x[i - 1];
                    splines[i].d = (splines[i].c - splines[i - 1].c)/hi;
                    splines[i].b = hi*(2.0*splines[i].c + splines[i - 1].c)/6.0 + (y[i] - y[i - 1])/hi;
                }
            }
            // Calculate value for interpolated function at an arbitrary point
            public double Interpolate(double x)
            {
                if (splines == null)
                {
                    return double.NaN; // If splines are not build yet -return NaN
                }

                int n = splines.Length;
                SplineTuple s;

                if (x <= splines[0].x) //If x less than x[0] - use first array element
                {
                    s = splines[0];
                }
                else if (x >= splines[n - 1].x)
                // If x greater than x[n-1] - use last array element
                {
                    s = splines[n - 1];
                }
                else
                // Otherwise, x is between the grid points - we produce a binary search for the appropriate array element
                {
                    int i = 0;
                    int j = n - 1;
                    while (i + 1 < j)
                    {
                        int k = i + (j - i)/2;
                        if (x <= splines[k].x)
                        {
                            j = k;
                        }
                        else
                        {
                            i = k;
                        }
                    }
                    s = splines[j];
                }

                double dx = x - s.x;
                //calculate spline value in set point by Horner's method
                double horner = s.a + (s.b + (s.c/2.0 + s.d*dx/6.0)*dx)*dx;
                double normal = s.a + (s.b * x) + (s.c * x * x) + (s.d * Math.Pow(x, 3));
                double diff = Math.Abs(horner - normal);
                //if (diff > 1e-8) throw new Exception($"diff = {diff}");
                return horner;
            }
        }
    }
}

