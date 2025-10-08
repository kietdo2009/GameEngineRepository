using System;

namespace Lab1._1
{
    public struct Fraction
    {
        private int numerator;
        private int denominator;

        // Constructor with default values (return 0)
        public Fraction(int n = 0, int d = 1)
        {
            numerator = n;
            denominator = d;

            // Ensure denominator is not zero
            if (denominator == 0)
                denominator = 1;

            Simplify();
        }

        // Public properties with simplification on set
        public int Numerator
        {
            get { return numerator; }
            set { numerator = value; Simplify(); }
        }

        public int Denominator
        {
            get { return denominator; }
            set
            {
                denominator = value;
                if (denominator == 0)
                    denominator = 1;
                Simplify();
            }
        }

        // Override ToString method in Lab1.1.cs
        public override string ToString()
        {
            return numerator + "/" + denominator;
        }

        // Simplify the fraction
        private void Simplify()
        {
            // Ensure denominator is positive if negative
            if (denominator < 0)
            {
                denominator *= -1;
                numerator *= -1;
            }

            // Find GCD and divide both numerator and denominator by it
            int gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
        }

        // Greatest Common Divisor using Euclid's algorithm
        private static int GCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b; // Returns whichever is not zero
        }

        // Operator overloads
        public static Fraction operator +(Fraction lhs, Fraction rhs)
        {
            int commonDenominator = lhs.denominator * rhs.denominator;
            int newNumerator = (lhs.numerator * rhs.denominator) + (rhs.numerator * lhs.denominator);
            return new Fraction(newNumerator, commonDenominator);
        }

        public static Fraction operator -(Fraction lhs, Fraction rhs)
        {
            int commonDenominator = lhs.denominator * rhs.denominator;
            int newNumerator = (lhs.numerator * rhs.denominator) - (rhs.numerator * lhs.denominator);
            return new Fraction(newNumerator, commonDenominator);
        }

        public static Fraction operator *(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.numerator, lhs.denominator * rhs.denominator);
        }

        public static Fraction operator /(Fraction lhs, Fraction rhs)
        {
            // Multiply by the reciprocal
            return new Fraction(lhs.numerator * rhs.denominator, lhs.denominator * rhs.numerator);
        }
    }
}