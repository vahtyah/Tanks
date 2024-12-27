using System.Linq;

public class GameModeSelector : SelectorCustomBase<GameMode>
{
    public override string GetTextOption(int index)
    {
        var text = options[index].ToString();
        var result = new System.Text.StringBuilder();
        foreach (var c in text.Where(char.IsUpper))
        {
            result.Append(c);
        }

        return result.ToString();
    }
}