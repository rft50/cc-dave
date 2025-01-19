using Jester.Cards;
using Jester.External;
using Microsoft.Xna.Framework;

namespace Jester.Render;

public class ZipperCardRenderManager : ICardRenderHook
{
    public Matrix ModifyNonTextCardRenderMatrix(G g, Card card, List<CardAction> actions)
    {
        if (card is not AbstractJoker || actions.Count < 6)
            return Matrix.Identity;
        return Matrix.CreateScale(1, 3f/4f, 1);
    }

    public Matrix ModifyCardActionRenderMatrix(G g, Card card, List<CardAction> actions, CardAction action, int actionWidth)
    {
        if (card is not AbstractJoker || actions.Count < 6)
            return Matrix.Identity;
        var index = actions.IndexOf(action);
        var parity = index % 2 == 0 ? 1 : -1;
        return Matrix.Multiply(
            Matrix.CreateScale(1, 4f/3f, 1),
            Matrix.CreateTranslation(parity * 32, 0, 0)
        );
    }
}