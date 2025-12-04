public class TestRSA {
    
    rsa test = new rsa();

    public static void main(String[] args) {
        TestRSA test = new TestRSA();
        test.testRSA();
    }
    
    public void testRSA(){
        int[] cle = test.ChoixCle(1000, 5000);
        int[] clePublique = test.ClePublic(cle[0], cle[1], cle[2]);
        int[] clePrivee = test.ClePrivee(cle[0], cle[1], cle[2]);

        int M = 42;
        int codage = test.CodageRSA(M, clePublique);
        int decodage = test.DecodageRSA(codage, clePrivee);
        System.out.println("Message original : " + M);
        System.out.println("Message codé : " + codage);
        System.out.println("Message décodé : " + decodage);
    }
}
