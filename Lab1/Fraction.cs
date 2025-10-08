namespace Lab1
{
    public struct Fraction
    {
        //Variables
        private int numerator;
        private int denominator;
        //Properties
        public int Numerator
        {
            get { return numerator; }
            set { numerator = value; Simplify(); }
        }
        public int Denominator
        {
            get { return denominator; }
            set { denominator = value; Simplify(); }
        }
        //Constructor
        public Fraction(int n = 0, int d = 1)
        {
            numerator = n;
            if (d == 0)
                d = 1;
            denominator = d;
        }
        public override String ToString()
        {
            return numerator + "/" + denominator;
        }
        public static Fraction operator *(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.numerator,
                            lhs.denominator * rhs.denominator);
        }
        public static Fraction Multiply(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.numerator,
                            lhs.denominator * rhs.denominator);
        }


        private void Simplify()
        {
            if (denominator < 0)
            {
                denominator *= -1;
                numerator *= -1;
            }
            int gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
        }

        private int GCD(int n, int d)
        {

            throw new NotImplementedException();
        }
    }
}