// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("RYItheE4Awqkgg3Ac+8Ne/3dSJBREEf9cDbnj0bRm8IfecPD2AR/9xHIffnt4Mw0E5YqwP6xl/ycrY2mHZ6Qn68dnpWdHZ6enxtjIc3EFIkNZDWFAUROnDMv/zSoGiV1uy+oxPK9DHTQKDqc5tLGu8b5ln89HMp8iCoXpyCS/fdLYkdVABb4jZsNU5CFCOnSO/ktXJzHtAKv72U4LPo5zeV6nipvU6O2cyV4ApIblDthxQfwaBHPtEs1cFyNXKp2TJvJtkInVYaEPgI53pD1lApWbHv1NQrHiAN354fLE6hyK4STOc9XxtL04thseXk4rx2eva+SmZa1GdcZaJKenp6an5xHuS9YMAsYkcEfXWb8RfaziGmpYTPPzURWm27FkJ2cnp+e");
        private static int[] order = new int[] { 8,9,13,9,12,5,13,7,11,12,10,13,13,13,14 };
        private static int key = 159;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
