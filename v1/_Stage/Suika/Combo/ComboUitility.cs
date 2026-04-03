using SOSG.Stage;

namespace SOSG.Stage.Suika.Combo
{
    public static class ComboUtility
    {
        public static ComboGrade GetGrade(int combo)
        {
            return combo switch
            {
                >= (int)ComboGrade.Extreme => ComboGrade.Extreme,
                >= (int)ComboGrade.High => ComboGrade.High,
                >= (int)ComboGrade.Mid => ComboGrade.Mid,
                >= (int)ComboGrade.Low => ComboGrade.Low,
                _ => ComboGrade.None
            };
        }

        public static bool IsCombo(int combo) => combo >= (int)ComboGrade.Low;
    }
}