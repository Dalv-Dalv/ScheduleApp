using SchoolApp;
using Xamarin.Forms;

public struct UrgencyColors {
    public string name;
    public Color backgroundColor, borderColor, textColor;

    public UrgencyColors(string name, Color backgroundColor, Color borderColor, Color textColor) {
        this.name = name;
        this.backgroundColor = backgroundColor;
        this.borderColor = borderColor;
        this.textColor = textColor;
    }

    //WHEN MODIFYING URGENCY COLORS ALSO MODIFY IN App.xaml
    public static UrgencyColors[] urgencyColors = {
        new UrgencyColors("None", Color.FromHex("#3B4026"),Color.FromHex("#8FA329"),Color.FromHex("#B2CC33")),
        new UrgencyColors("Orange", Color.FromHex("#472C1F"),Color.FromHex("#F2590D"),Color.FromHex("#FF7733")),
        new UrgencyColors("Red", Color.FromHex("#402626"),Color.FromHex("#D92626"),Color.FromHex("#EB4747"))
    };
}