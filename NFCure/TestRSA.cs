public class TestRSA {
    
    rsa test = new rsa();

    public static void Main(String[] args)
    {
        TestRSA test = TestRSA();
        test.testRSA();
    }
    
    public void TestRSA()
    {
        int[] cle = test.ChoixCle(1000, 5000);
        int[] clePublique = test.ClePublic(cle[0], cle[1], cle[2]);
        int[] clePrivee = test.ClePrivee(cle[0], cle[1], cle[2]);

        int M = 42;
        int codage = test.CodageRSA(M, clePublique);
        int decodage = test.DecodageRSA(codage, clePrivee);
    }
}
