public class RSA
{
    public int pgcd(int a, int b)
    {
        a = Math.abs(a);
        b = Math.abs(b);
        if (a == 0) return b;
        if (b == 0) return a;
        while (b != 0)
        {
            int t = a % b;
            a = b;
            b = t;
        }
        return a;
    }
    
    public int InverseModulaire(int e, int m)
    {
        //d = (1 + km) / e
        double k = 0;
        while (true)
        {
            double d = (1 + k * m) / e;
            if (d == Math.floor(d))
            {
                if (EstPremierEntreEux((int)d, m))
                {
                    return (int)d;
                }
            }
            k++;
        }
    }

    public boolean EstPremierEntreEux(int a, int b)
    {
        return pgcd(a, b) == 1;
    }

    public boolean EstPremier(int a)
    {
        if (a <= 1) return false;
        if (a % 2 == 0) return false;
        for (int i = 3; i <= Math.sqrt(a); i += 2)
        {
            if (a % i == 0) return false;
        }
        return true;
    }

    public int PremierAleatoire(int inf, int lg)
    {
        ArrayList<Integer> premiers = new ArrayList<>();
        for (int i = inf ; i < lg; i++)
        {
            if (EstPremier(i))
            {
                premiers.add(i);
            }
        }
        if (premiers.size() > 0)
        {
            return premiers.get((int)(Math.random() * premiers.size()));
        }
        return -1;
    }

    public int PremierAleatoireAvec(int n)
    {
        ArrayList<Integer> premiers = new ArrayList<>();
        for (int i = 2 ; i < n; i++)
        {
            if (EstPremierEntreEux(i, n))
            {
                premiers.add(i);
            }
        }
        if (premiers.size() > 0)
        {
            return premiers.get((int)(Math.random() * premiers.size()));
        }
        return -1;
    }

    public int ExpoModulaire(int a, int n, int m)
    {
        if (m == 1) return 0;
        if (n < 0) throw new IllegalArgumentException("Negative exponent not supported");
        long start = ((long)a % m + m) % m;
        long result = 1 % m;
        while (n > 0)
        {
            if ((n & 1) == 1)
            {
                result = result * start % m;
            }
            start = start * start % m;
            n >>= 1;
        }
        return (int) result;
    }



    // codage rsa
    public int[] ChoixCle(int inf, int lg)
    {
        int p = PremierAleatoire(inf, lg);
        int q = PremierAleatoire(p + 1, p + lg + 1);
        int e = PremierAleatoireAvec((p - 1) * (q - 1));
        return [p, q, e];
    }

    public int[] ClePublic(int p, int q, int e)
    {
        int n = p * q;
        return [n, e];
    }

    public int[] ClePrivee(int p, int q, int e)
    {
        int n = p * q;
        int d = InverseModulaire(e, (p - 1) * (q - 1));
        return [n, d];
    }

    public int CodageRSA(int M, int[] clePublique)
    {
        int n = clePublique[0];
        int e = clePublique[1];
        if (M >= n) return -1;
        return ExpoModulaire(M, e, n);
    }

    public int DecodageRSA(int M, int[] clePrivee)
    {
        int n = clePrivee[0];
        int d = clePrivee[1];
        if (M >= n) return -1;
        return ExpoModulaire(M, d, n);
    }
}